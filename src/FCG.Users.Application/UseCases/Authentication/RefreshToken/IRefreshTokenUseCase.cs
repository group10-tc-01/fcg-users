using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Authentication.RefreshToken
{
    public interface IRefreshTokenUseCase : ICommandHandler<RefreshTokenRequest, RefreshTokenResponse> { }
}