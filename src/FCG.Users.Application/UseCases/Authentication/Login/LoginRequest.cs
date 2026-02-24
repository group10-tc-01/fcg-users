using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public record class LoginRequest(string Email, string Password) : ICommand<Result<LoginResponse>>;
}
