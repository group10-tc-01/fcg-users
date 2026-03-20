using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.RefreshTokens;
using FCG.Users.Domain.Users.Events;
using FCG.Users.Domain.Users.ValueObjects;

namespace FCG.Users.Domain.Users
{
    public sealed class User : BaseEntity, IAuditableEntity
    {
        public Name Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public Role Role { get; private set; }
        public ICollection<RefreshToken>? RefreshTokens { get; }

        #region Audits properties
        // Implementação explícita - expõe via interface, mas mantém protected set
        DateTime IAuditableEntity.CreatedAt { get => CreatedAt; set => CreatedAt = value; }
        DateTime? IAuditableEntity.UpdatedAt { get => UpdatedAt; set => UpdatedAt = value; }
        string IAuditableEntity.CreatedBy { get => CreatedBy; set => CreatedBy = value; }
        string? IAuditableEntity.UpdatedBy { get => UpdatedBy; set => UpdatedBy = value; }
        #endregion

        public static User CreateRegularUser(string name, string email, string password)
        {
            var user = new User(name, email, password, Role.User);

            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Name, user.Email));

            return user;
        }

        public static User CreateAdminUser(string name, string email, string password)
        {
            var user = new User(name, email, password, Role.Admin);

            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Name, user.Email));

            return user;
        }

        public void UpdatePassword(string password)
        {
            Password = Password.CreateFromHash(password);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRole(Role newRole)
        {
            Role = newRole;
            UpdatedAt = DateTime.UtcNow;
        }

        private User(Name name, Email email, Password password, Role role) : base(Guid.NewGuid())
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        private User() { }
    }
}
