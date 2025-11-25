using MediatR;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public interface IRegisterUserUseCase : IRequestHandler<RegisterUserRequest, RegisterUserResponse> { }
}
