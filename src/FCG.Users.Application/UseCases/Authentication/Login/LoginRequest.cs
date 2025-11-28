using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public record class LoginRequest(string Email, string Password) : ICommand<LoginResponse>;
}
