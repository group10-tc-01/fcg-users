using FCG.Users.Domain.Users;

namespace FCG.Users.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task CreateRefreshTokenAsync(string token, Guid userId);
        Task<string?> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}