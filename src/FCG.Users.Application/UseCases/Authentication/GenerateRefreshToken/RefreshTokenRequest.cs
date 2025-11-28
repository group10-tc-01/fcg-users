using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public record RefreshTokenRequest(string RefreshToken) : ICommand<RefreshTokenResponse>;
}