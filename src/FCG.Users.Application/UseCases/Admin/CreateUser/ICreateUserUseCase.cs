using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Admin.CreateUser
{
    public interface ICreateUserUseCase : ICommandHandler<CreateUserRequest, CreateUserResponse> { }
}
