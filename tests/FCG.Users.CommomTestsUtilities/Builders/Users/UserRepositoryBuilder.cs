using FCG.Users.Domain.Users;
using Moq;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public static class UserRepositoryBuilder
    {
        public static readonly Mock<IUserRepository> _mock = new Mock<IUserRepository>();

        public static IUserRepository Build() => _mock.Object;

        public static void SetupGetByEmailAsync(User? user)
        {
            _mock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        }

        public static void SetupGetByIdAsync(User? user)
        {
            _mock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        }

        public static void SetupGetPagedUsersAsync(IEnumerable<User> users, int totalCount)
        {
            _mock.Setup(repo => repo.GetPagedUsersAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>())).ReturnsAsync((users, totalCount));
        }
    }
}
