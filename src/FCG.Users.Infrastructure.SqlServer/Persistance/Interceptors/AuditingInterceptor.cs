using FCG.Users.Application.Abstractions;
using FCG.Users.Application.Abstractions.Audit;
using FCG.Users.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FCG.Users.Infrastructure.SqlServer.Persistance.Interceptors
{
    public class AuditingInterceptor(ICurrentSessionProvider currentSessionProvider, ILogger<AuditingInterceptor> logger) : SaveChangesInterceptor
    {
        private readonly ICurrentSessionProvider _currentSessionProvider = currentSessionProvider;
        private readonly ILogger<AuditingInterceptor> _logger = logger;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is FcgUserDbContext context)
            {
                context.ChangeTracker.DetectChanges();

                var auditEntries = GetAuditEntries(context);

                foreach (var entry in auditEntries)
                {
                    context.AuditTrail.Add(entry);
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private List<AuditTrail> GetAuditEntries(FcgUserDbContext context)
        {
            var auditEntries = new List<AuditTrail>();
            var userId = _currentSessionProvider.GetUserId();

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.Entity is not IAuditableEntity)
                    continue;

                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var entityName = entry.Entity.GetType().Name;
                var primaryKey = entry.Entity.Id.ToString();
                var trailType = entry.State switch
                {
                    EntityState.Added => AuditTrailType.Create,
                    EntityState.Modified => AuditTrailType.Update,
                    EntityState.Deleted => AuditTrailType.Delete,
                    _ => (AuditTrailType?)null
                };

                if (trailType is null)
                {
                    _logger.LogWarning(
                        "[AuditingInterceptor] Failed to determine audit trail type for entity {EntityName} (ID: {EntityId}). " +
                        "Entity state {EntityState} is not supported.",
                        entityName,
                        primaryKey,
                        entry.State);
                    continue;
                }

                var ownedEntries = GetOwnedEntries(entry);
                var oldValues = GetOldValuesAsJson(entry, ownedEntries);
                var newValues = GetNewValuesAsJson(entry, ownedEntries);
                var changedColumns = GetChangedColumnsAsJson(entry, ownedEntries);

                var auditTrail = new AuditTrail(
                    userId,
                    entityName,
                    primaryKey,
                    trailType.Value,
                    oldValues,
                    newValues,
                    changedColumns);

                auditEntries.Add(auditTrail);
            }

            return auditEntries;
        }

        private static string GetOldValuesAsJson(EntityEntry<BaseEntity> entry, IReadOnlyCollection<OwnedEntryInfo> ownedEntries)
        {
            if (entry.State != EntityState.Modified && entry.State != EntityState.Deleted)
                return string.Empty;

            var values = BuildAuditSnapshot(entry, ownedEntries, useOriginalValues: true, includeAll: entry.State == EntityState.Deleted);

            return values.Count > 0 ? JsonSerializer.Serialize(values) : string.Empty;
        }

        private static string GetNewValuesAsJson(EntityEntry<BaseEntity> entry, IReadOnlyCollection<OwnedEntryInfo> ownedEntries)
        {
            if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                return string.Empty;

            var values = BuildAuditSnapshot(entry, ownedEntries, useOriginalValues: false, includeAll: entry.State == EntityState.Added);

            return values.Count > 0 ? JsonSerializer.Serialize(values) : string.Empty;
        }

        private static string GetChangedColumnsAsJson(EntityEntry<BaseEntity> entry, IReadOnlyCollection<OwnedEntryInfo> ownedEntries)
        {
            if (entry.State != EntityState.Modified)
                return string.Empty;

            var changed = new List<string>();

            changed.AddRange(entry.Properties
                .Where(p => !p.IsTemporary && !p.Metadata.IsForeignKey() && !Equals(p.OriginalValue, p.CurrentValue))
                .Select(p => p.Metadata.Name));

            foreach (var owned in ownedEntries)
            {
                changed.AddRange(owned.Entry.Properties
                    .Where(p => !p.IsTemporary && !p.Metadata.IsForeignKey() && !p.Metadata.IsPrimaryKey())
                    .Where(p => !Equals(ResolveOriginalValue(entry, p), p.CurrentValue))
                    .Select(p => BuildPropertyName(owned.NavigationName, p)));
            }

            return changed.Count > 0 ? JsonSerializer.Serialize(changed) : string.Empty;
        }

        private static Dictionary<string, object?> BuildAuditSnapshot(
            EntityEntry<BaseEntity> entry,
            IReadOnlyCollection<OwnedEntryInfo> ownedEntries,
            bool useOriginalValues,
            bool includeAll)
        {
            var values = new Dictionary<string, object?>();

            CollectOwnerProperties(values, entry, useOriginalValues, includeAll);

            foreach (var owned in ownedEntries)
            {
                CollectOwnedProperties(values, entry, owned, useOriginalValues, includeAll);
            }

            return values;
        }

        private static void CollectOwnerProperties(
            IDictionary<string, object?> values,
            EntityEntry<BaseEntity> entry,
            bool useOriginalValues,
            bool includeAll)
        {
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary || property.Metadata.IsForeignKey())
                    continue;

                if (!includeAll && Equals(property.OriginalValue, property.CurrentValue))
                    continue;

                values[BuildPropertyName(null, property)] = useOriginalValues
                    ? property.OriginalValue
                    : property.CurrentValue;
            }
        }

        private static void CollectOwnedProperties(
            IDictionary<string, object?> values,
            EntityEntry<BaseEntity> ownerEntry,
            OwnedEntryInfo owned,
            bool useOriginalValues,
            bool includeAll)
        {
            foreach (var property in owned.Entry.Properties)
            {
                // Skip internal infrastructure properties of owned types:
                // - IsForeignKey(): explicit FK columns
                // - IsPrimaryKey(): the shadow PK of table-split owned entities is the same
                //   column as the owner's PK (e.g. Email.UserId, Password.UserId) and must
                //   be excluded; IsForeignKey() alone does NOT catch it in EF Core 9.
                if (property.IsTemporary || property.Metadata.IsForeignKey() || property.Metadata.IsPrimaryKey())
                    continue;

                // For owned record types that are replaced (new instance assigned), EF Core
                // may re-track the owned entry without a proper OriginalValue snapshot.
                // The owner entry always has the correct snapshot for all columns in its table,
                // so we resolve the original value from the owner when the owned entry's
                // OriginalValue appears to be missing (equal to CurrentValue and the owner
                // entry itself shows the column as changed).
                var originalValue = ResolveOriginalValue(ownerEntry, property);

                if (!includeAll && Equals(originalValue, property.CurrentValue))
                    continue;

                values[BuildPropertyName(owned.NavigationName, property)] = useOriginalValues
                    ? originalValue
                    : property.CurrentValue;
            }
        }

        /// <summary>
        /// Resolves the original (database-loaded) value for a property that belongs to an owned type.
        /// When a record-type owned entity is replaced wholesale, EF Core may re-track the owned entry
        /// without a valid OriginalValue snapshot. In that case, we fall back to the owner entry's
        /// OriginalValues keyed by the property's IProperty metadata, which always retains the
        /// database snapshot for all columns in the owner's table.
        /// </summary>
        private static object? ResolveOriginalValue(EntityEntry ownerEntry, PropertyEntry ownedProperty)
        {
            // If the owned entry has a valid original value (differs from current, or owner isn't Modified),
            // trust it directly.
            if (!Equals(ownedProperty.OriginalValue, ownedProperty.CurrentValue))
                return ownedProperty.OriginalValue;

            // Fall back to the owner entry's snapshot, which is keyed by IProperty.
            // This works because table-split owned type properties are columns of the same table.
            try
            {
                return ownerEntry.OriginalValues[ownedProperty.Metadata];
            }
            catch
            {
                // Property not found in owner's OriginalValues (e.g. navigations to separate tables).
                return ownedProperty.OriginalValue;
            }
        }

        private static IReadOnlyCollection<OwnedEntryInfo> GetOwnedEntries(EntityEntry<BaseEntity> ownerEntry)
        {
            // Owned types with OwnsOne in table-splitting (same table as owner).
            // We enumerate via References to get all owned navigations regardless of their entry state.
            return ownerEntry.References
                .Where(r => r.TargetEntry?.Metadata.IsOwned() == true)
                .Select(r => new OwnedEntryInfo(r.Metadata.Name, r.TargetEntry!))
                .ToList();
        }

        private static string BuildPropertyName(string? prefix, PropertyEntry property)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return property.Metadata.Name;

            // Value objects com propriedade unica "Value" sao expostos pelo nome da navegacao:
            // Password.Value => "Password", Email.Value => "Email"
            return property.Metadata.Name == "Value"
                ? prefix
                : $"{prefix}.{property.Metadata.Name}";
        }

        private sealed record OwnedEntryInfo(string NavigationName, EntityEntry Entry);
    }
}
