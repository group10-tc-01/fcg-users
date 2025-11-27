using MediatR;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public record class LoginRequest(string Email, string Password) : IRequest<LoginResponse>;
}
