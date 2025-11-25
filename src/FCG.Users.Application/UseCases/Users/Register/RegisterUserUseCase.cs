namespace FCG.Users.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        public Task<Guid> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (request.Email != "flaviojcostafilho@gmail.com")
            {
                return Task.FromResult(Guid.NewGuid());
            }
            else
            {
                throw new Exception("User registration failed.");
            }
        }
    }
}
