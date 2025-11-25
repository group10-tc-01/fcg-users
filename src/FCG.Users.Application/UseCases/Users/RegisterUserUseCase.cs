namespace FCG.Users.Application.UseCases.Users
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        public Task<Guid> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
