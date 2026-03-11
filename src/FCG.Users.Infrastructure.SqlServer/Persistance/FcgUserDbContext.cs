using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.RefreshTokens;
using FCG.Users.Domain.Users;
using FCG.Users.Infrastructure.SqlServer.Persistance.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.SqlServer.Persistance
{
    [ExcludeFromCodeCoverage]
    public class FcgUserDbContext : DbContext, IUnitOfWork
    {
        private readonly IPublisher _publisher;
        private readonly AuditingInterceptor _auditingInterceptor;

        public DbSet<User> User { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }

        public FcgUserDbContext(
            DbContextOptions<FcgUserDbContext> options,
            IPublisher publisher,
            AuditingInterceptor auditingInterceptor) : base(options)
        {
            _publisher = publisher;
            _auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditingInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgUserDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEventsAsync();

            return result;
        }

        private async Task PublishDomainEventsAsync()
        {
            var domainEvents = ChangeTracker
                               .Entries<BaseEntity>()
                               .Select(entry => entry.Entity)
                               .SelectMany(entity =>
                               {
                                   var domainEvents = entity.GetDomainEvents();

                                   entity.ClearDomainEvents();

                                   return domainEvents;
                               }).ToList();

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}
