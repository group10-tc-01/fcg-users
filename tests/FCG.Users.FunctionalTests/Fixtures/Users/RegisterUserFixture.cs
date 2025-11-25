using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.CommomTestsUtilities.Builders.Users;

namespace FCG.Users.FunctionalTests.Fixtures.Users
{
    public class RegisterUserFixture
    {
        public RegisterUserUseCase RegisterUserUseCase { get; }
        public RegisterUserRequest RegisterUserRequest { get; }

        public RegisterUserFixture()
        {
            RegisterUserUseCase = new RegisterUserUseCase();
            RegisterUserRequest = new RegisterUserRequestBuilder().Build();
        }
    }
}
