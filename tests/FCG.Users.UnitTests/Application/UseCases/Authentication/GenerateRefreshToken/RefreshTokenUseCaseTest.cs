using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Settings;
using FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

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
            response.IsSuccess.Should().BeTrue();
            response.Value!.AccessToken.Should().Be(newAccessToken);
            response.Value.RefreshToken.Should().Be(newRefreshToken);
            response.Value.ExpiresInDays.Should().Be(_jwtSettings.Value.RefreshTokenExpirationDays);
        }

        [Fact]
        public async Task Given_InvalidRefreshToken_When_Handle_Then_ShouldReturnUnauthorizedFailure()
        {
            // Arrange
            var request = new RefreshTokenRequestBuilder().Build();
            AuthenticationServiceBuilder.SetupValidateRefreshTokenAsync(null);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.ErrorMessage.Should().Be(ResourceMessages.InvalidRefreshToken);
        }

        [Fact]
        public async Task Given_ValidRefreshTokenButUserNotFound_When_Handle_Then_ShouldReturnUnauthorizedFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new RefreshTokenRequestBuilder().Build();
            AuthenticationServiceBuilder.SetupValidateRefreshTokenAsync(userId.ToString());
            UserRepositoryBuilder.SetupGetByIdAsync(null);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.ErrorMessage.Should().Be(ResourceMessages.InvalidRefreshToken);
        }
    }
}
