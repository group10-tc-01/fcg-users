using MediatR;

namespace FCG.Users.Application.UseCases.Users
{
    public record RegisterUserRequest(string Name, string Email, string Password) : IRequest<Guid>;
}
