using FCG.Users.Domain.Users;

namespace FCG.Users.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        string GenerateAccessToken(User user);
    }
}