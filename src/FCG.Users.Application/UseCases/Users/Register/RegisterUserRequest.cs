using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public record RegisterUserRequest(string Name, string Email, string Password) : ICommand<RegisterUserResponse>;
}
