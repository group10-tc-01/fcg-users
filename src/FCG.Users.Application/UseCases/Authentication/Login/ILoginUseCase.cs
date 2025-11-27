using MediatR;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public interface ILoginUseCase : IRequestHandler<LoginRequest, LoginResponse> { }
}
