using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public interface IUpdateUserUseCase : ICommandHandler<UpdatePasswordRequest, Result<UpdatePasswordResponse>> { }
}
