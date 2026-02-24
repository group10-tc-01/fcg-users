using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;
using FCG.Users.Domain.Users;

namespace FCG.Users.Application.UseCases.Admin.CreateUser
{
    public record CreateUserRequest(string Name, string Email, string Password, Role Role) : ICommand<Result<CreateUserResponse>>;
}
