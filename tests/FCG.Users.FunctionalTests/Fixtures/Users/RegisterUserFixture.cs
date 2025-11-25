using FCG.Users.Application.UseCases.Users.Register;
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

        public RegisterUserFixture()
        {
            var userRepository = new Mock<IUserRepository>().Object;
            var unitOfWork = new Mock<IUnitOfWork>().Object;

            RegisterUserUseCase = new RegisterUserUseCase(userRepository, unitOfWork);
            RegisterUserRequest = new RegisterUserRequestBuilder().Build();
        }
    }
}
