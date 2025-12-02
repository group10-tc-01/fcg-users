using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public interface IUpdateUserUseCase : ICommandHandler<UpdatePasswordRequest, UpdatePasswordResponse> { }
}
