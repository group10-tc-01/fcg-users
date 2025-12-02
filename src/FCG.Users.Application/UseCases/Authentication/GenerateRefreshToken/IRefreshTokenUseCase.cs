using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public interface IRefreshTokenUseCase : ICommandHandler<RefreshTokenRequest, RefreshTokenResponse> { }
}