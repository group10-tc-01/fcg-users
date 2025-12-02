using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Admin.DeactivateUser
{
    public record DeactivateUserRequest(Guid Id) : ICommand<DeactivateUserResponse>;

}
