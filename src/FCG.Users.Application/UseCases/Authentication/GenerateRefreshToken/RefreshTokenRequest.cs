using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public record RefreshTokenRequest(string RefreshToken) : ICommand<Result<RefreshTokenResponse>>;
}
