using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Admin.DeactivateUser
{
    public interface IDeactivateUserUseCase : ICommandHandler<DeactivateUserRequest, DeactivateUserResponse>
    {
    }
}
