namespace FCG.Users.Application.UseCases.Authentication.RefreshToken
{
    public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresInDays);
}