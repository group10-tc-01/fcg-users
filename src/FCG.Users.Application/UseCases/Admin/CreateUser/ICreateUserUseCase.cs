using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Admin.CreateUser
{
    public interface ICreateUserUseCase : ICommandHandler<CreateUserRequest, Result<CreateUserResponse>> { }
}
