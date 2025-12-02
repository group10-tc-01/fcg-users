using FCG.Users.Application.Settings;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.RefreshTokens;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.RefreshTokens;
using FCG.Users.Infrastructure.Auth.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Users.UnitTests.Auth
{
    public class AuthenticationServiceTest
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly AuthenticationService _sut;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationServiceTest()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _unitOfWorkMock = UnitOfWorkBuilder.Build() as Mock<IUnitOfWork> ?? new Mock<IUnitOfWork>();

            _jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsAVerySecureSecretKeyForTestingPurposesWithAtLeast32Characters!",
                Issuer = "FCG.Users.Test",
                Audience = "FCG.Users.Test.Audience",
                AccessTokenExpirationMinutes = 60,
                RefreshTokenExpirationDays = 7
            };

            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

            _sut = new AuthenticationService(_jwtSettingsMock.Object, _refreshTokenRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void Given_GenerateRefreshToken_When_CalledMultipleTimes_Then_ShouldReturnDifferentTokens()
        {
            // Act
            var token1 = _sut.GenerateRefreshToken();
            var token2 = _sut.GenerateRefreshToken();

            // Assert
            token1.Should().NotBe(token2);
        }

        [Fact]
        public async Task Given_ValidTokenAndUserId_When_CreateRefreshTokenAsync_Then_ShouldSaveRefreshToken()
        {
            // Arrange
            var token = "test-refresh-token";
            var userId = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _sut.CreateRefreshTokenAsync(token, userId);

            // Assert
            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.Is<RefreshToken>(rt =>
                rt.Token == token && rt.UserId == userId)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Given_ValidRefreshToken_When_ValidateRefreshTokenAsync_Then_ShouldReturnUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = new RefreshTokenBuilder().BuildWithUserId(userId);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _sut.ValidateRefreshTokenAsync(refreshToken.Token);

            // Assert
            result.Should().Be(userId.ToString());
        }

        [Fact]
        public async Task Given_ExpiredRefreshToken_When_ValidateRefreshTokenAsync_Then_ShouldReturnNull()
        {
            // Arrange
            var expiredToken = new RefreshTokenBuilder().BuildExpired();
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(expiredToken);

            // Act
            var result = await _sut.ValidateRefreshTokenAsync(expiredToken.Token);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Given_NonExistentRefreshToken_When_ValidateRefreshTokenAsync_Then_ShouldReturnNull()
        {
            // Arrange
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _sut.ValidateRefreshTokenAsync("non-existent-token");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Given_ValidRefreshToken_When_RevokeRefreshTokenAsync_Then_ShouldRevokeAndSave()
        {
            // Arrange
            var refreshToken = new RefreshTokenBuilder().Build();
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(refreshToken);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _sut.RevokeRefreshTokenAsync(refreshToken.Token);

            // Assert
            refreshToken.IsValid.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Given_NonExistentRefreshToken_When_RevokeRefreshTokenAsync_Then_ShouldNotThrow()
        {
            // Arrange
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var act = async () => await _sut.RevokeRefreshTokenAsync("non-existent-token");

            // Assert
            await act.Should().NotThrowAsync();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
