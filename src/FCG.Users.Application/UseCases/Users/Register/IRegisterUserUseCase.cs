using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public interface IRegisterUserUseCase : ICommandHandler<RegisterUserRequest, RegisterUserResponse> { }
}
