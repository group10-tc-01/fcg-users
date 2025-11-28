using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public interface ILoginUseCase : ICommandHandler<LoginRequest, LoginResponse> { }
}
