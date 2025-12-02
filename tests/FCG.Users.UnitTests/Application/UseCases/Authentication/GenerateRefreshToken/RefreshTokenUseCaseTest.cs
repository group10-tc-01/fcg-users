using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Settings;
using FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Users.UnitTests.Application.UseCases.Authentication.GenerateRefreshToken
{
    public class RefreshTokenUseCaseTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<RefreshTokenUseCase> _logger;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IRefreshTokenUseCase _sut;

        public RefreshTokenUseCaseTest()
        {
            _userRepository = UserRepositoryBuilder.Build();
            _authenticationService = AuthenticationServiceBuilder.Build();
            _logger = new Mock<ILogger<RefreshTokenUseCase>>().Object;
            _jwtSettings = Options.Create(new JwtSettings
            {
                SecretKey = "test-secret-key-with-minimum-256-bits-length-required",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            });
            _sut = new RefreshTokenUseCase(_userRepository, _authenticationService, _logger, _jwtSettings);
        }

        [Fact]
        public async Task Given_ValidRefreshToken_When_Handle_Then_ShouldRefreshTokenSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new RefreshTokenRequestBuilder().Build();
            var newAccessToken = "new-access-token";
            var newRefreshToken = "new-refresh-token";

            AuthenticationServiceBuilder.SetupValidateRefreshTokenAsync(user.Id.ToString());
            UserRepositoryBuilder.SetupGetByIdAsync(user);
            AuthenticationServiceBuilder.SetupRevokeRefreshTokenAsync();
            AuthenticationServiceBuilder.SetupGenerateAccessToken(newAccessToken);
            AuthenticationServiceBuilder.SetupGenerateRefreshToken(newRefreshToken);
            AuthenticationServiceBuilder.SetupCreateRefreshTokenAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().Be(newAccessToken);
            response.RefreshToken.Should().Be(newRefreshToken);
            response.ExpiresInDays.Should().Be(_jwtSettings.Value.RefreshTokenExpirationDays);
        }

        [Fact]
        public async Task Given_InvalidRefreshToken_When_Handle_Then_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var request = new RefreshTokenRequestBuilder().Build();
            AuthenticationServiceBuilder.SetupValidateRefreshTokenAsync(null);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>().WithMessage(ResourceMessages.InvalidRefreshToken);
        }

        [Fact]
        public async Task Given_ValidRefreshTokenButUserNotFound_When_Handle_Then_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new RefreshTokenRequestBuilder().Build();
            AuthenticationServiceBuilder.SetupValidateRefreshTokenAsync(userId.ToString());
            UserRepositoryBuilder.SetupGetByIdAsync(null);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>().WithMessage(ResourceMessages.InvalidRefreshToken);
        }
    }
}
