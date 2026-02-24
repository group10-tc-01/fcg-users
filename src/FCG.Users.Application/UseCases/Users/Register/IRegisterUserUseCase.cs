using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public interface IRegisterUserUseCase : ICommandHandler<RegisterUserRequest, Result<RegisterUserResponse>> { }
}
