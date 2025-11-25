using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Users.Events;
using FCG.Users.Domain.Users.ValueObjects;

namespace FCG.Users.Domain.Users
{
    public sealed class User : BaseEntity
    {
        public Name Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public Role Role { get; private set; }

        private User(Name name, Email email, Password password, Role role) : base(Guid.NewGuid())
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        private User() { }

        public static User Create(string name, string email, string password, Role role)
        {
            var user = new User(name, email, password, role);

            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Name, user.Email));

            return user;
        }
    }
}
