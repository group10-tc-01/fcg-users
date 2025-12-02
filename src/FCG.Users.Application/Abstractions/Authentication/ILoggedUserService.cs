using FCG.Users.Domain.Users;

namespace FCG.Users.Application.Abstractions.Authentication
{
    public interface ILoggedUserService
    {
        Task<User> GetLoggedUserAsync();
    }
}
