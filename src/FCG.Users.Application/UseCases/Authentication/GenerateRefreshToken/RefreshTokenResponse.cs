namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresInDays);
}