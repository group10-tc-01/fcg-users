using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Users;
using Moq;

namespace FCG.Users.FunctionalTests.Fixtures.Users
{
    public class RegisterUserFixture
    {
        public RegisterUserUseCase RegisterUserUseCase { get; }
        public RegisterUserRequest RegisterUserRequest { get; }
        public IPasswordEncrypterService PasswordEncrypter { get; }

        public RegisterUserFixture()
        {
            var userRepository = new Mock<IUserRepository>().Object;
            var unitOfWork = new Mock<IUnitOfWork>().Object;
            PasswordEncrypter = PasswordEncrypterServiceBuilder.Build();
            PasswordEncrypterServiceBuilder.SetupEncrypt();

            RegisterUserUseCase = new RegisterUserUseCase(userRepository, unitOfWork, PasswordEncrypter);
            RegisterUserRequest = new RegisterUserRequestBuilder().Build();
        }
    }
}
