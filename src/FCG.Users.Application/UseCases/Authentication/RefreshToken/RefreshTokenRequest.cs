using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.RefreshToken
{
    public record RefreshTokenRequest(string RefreshToken) : ICommand<RefreshTokenResponse>;
}