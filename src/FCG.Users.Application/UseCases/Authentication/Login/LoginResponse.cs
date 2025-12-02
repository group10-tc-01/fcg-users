namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public record LoginResponse(string AccessToken, string RefreshToken, int ExpiresInMinutes);
}
