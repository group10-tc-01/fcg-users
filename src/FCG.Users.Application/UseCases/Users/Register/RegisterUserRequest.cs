using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public record RegisterUserRequest(string Name, string Email, string Password) : ICommand<Result<RegisterUserResponse>>;
}
