using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public interface IRefreshTokenUseCase : ICommandHandler<RefreshTokenRequest, Result<RefreshTokenResponse>> { }
}
