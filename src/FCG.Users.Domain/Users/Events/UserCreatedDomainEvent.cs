using FCG.Users.Domain.Abstractions;

namespace FCG.Users.Domain.Users.Events
{
    public record UserCreatedDomainEvent(Guid UserId, string Name, string Email) : IDomainEvent;
}
