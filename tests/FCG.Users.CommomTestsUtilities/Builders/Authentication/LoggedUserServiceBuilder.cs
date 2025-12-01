using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Domain.Users;
using Moq;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public static class LoggedUserServiceBuilder
    {
        private static readonly Mock<ILoggedUserService> _mock = new Mock<ILoggedUserService>();

        public static ILoggedUserService Build() => _mock.Object;

        public static void SetupGetLoggedUserAsync(User user)
        {
            _mock.Setup(service => service.GetLoggedUserAsync()).ReturnsAsync(user);
        }
    }
}
