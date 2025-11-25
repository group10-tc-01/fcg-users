using MediatR;

namespace FCG.Users.Application.UseCases.Users
{
    public interface IRegisterUserUseCase : IRequestHandler<RegisterUserRequest, Guid> { }
}
