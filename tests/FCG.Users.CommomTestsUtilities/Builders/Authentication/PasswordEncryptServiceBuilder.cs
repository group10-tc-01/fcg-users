using FCG.Users.Application.Abstractions.Authentication;
using Moq;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public static class PasswordEncrypterServiceBuilder
    {
        private static readonly Mock<IPasswordEncrypterService> _mock = new Mock<IPasswordEncrypterService>();
        public static IPasswordEncrypterService Build() => _mock.Object;

        public static void SetupIsValid(bool isValid)
        {
            _mock.Setup(service => service.IsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(isValid);
        }

        public static void SetupEncrypt(string? password = null)
        {
            _mock.Setup(service => service.Encrypt(It.IsAny<string>())).Returns(password ?? "YourPassword123!");
        }
    }
}
