using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Settings;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken
{
    public class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<RefreshTokenUseCase> _logger;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenUseCase(
            IUserRepository userRepository,
            IAuthenticationService authenticationService,
            ILogger<RefreshTokenUseCase> logger,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {

            var userId = await _authenticationService.ValidateRefreshTokenAsync(request.RefreshToken);

            if (userId is null)
            {
                _logger.LogWarning("[RefreshTokenUseCase] Invalid refresh token");

                throw new UnauthorizedException(ResourceMessages.InvalidRefreshToken);
            }

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("[RefreshTokenUseCase]  User not found for userId: {UserId}", userId);

                throw new UnauthorizedException(ResourceMessages.InvalidRefreshToken);
            }

            await _authenticationService.RevokeRefreshTokenAsync(request.RefreshToken);

            var accessToken = _authenticationService.GenerateAccessToken(user);
            var newRefreshTokenValue = _authenticationService.GenerateRefreshToken();
            await _authenticationService.CreateRefreshTokenAsync(newRefreshTokenValue, user.Id);

            _logger.LogInformation("[RefreshTokenUseCase] Successfully refreshed token for user: {UserId}", user.Id);

            return new RefreshTokenResponse(accessToken, newRefreshTokenValue, _jwtSettings.RefreshTokenExpirationDays);
        }
    }
}