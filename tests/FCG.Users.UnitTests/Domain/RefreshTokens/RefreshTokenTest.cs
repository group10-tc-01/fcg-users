using FCG.Users.CommomTestsUtilities.Builders.RefreshTokens;
using FCG.Users.Messages;
using FluentAssertions;

namespace FCG.Users.UnitTests.Domain.RefreshTokens
{
    public class RefreshTokenTest
    {
        [Fact]
        public void Given_ValidRefreshToken_When_Instantiate_Then_ShouldCreateRefreshToken()
        {
            // Arrange & Act
            var refreshToken = new RefreshTokenBuilder().Build();

            // Assert
            refreshToken.Should().NotBeNull();
            refreshToken.Id.Should().NotBe(Guid.Empty);
            refreshToken.Token.Should().NotBeNullOrEmpty();
            refreshToken.Token.Length.Should().Be(30);
            refreshToken.UserId.Should().NotBe(Guid.Empty);
            refreshToken.User.Should().BeNull();
            refreshToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            refreshToken.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Given_ValidRefreshToken_When_Revoke_Then_ShouldRevokeRefreshToken()
        {
            // Arrange
            var refreshToken = new RefreshTokenBuilder().Build();

            // Act
            refreshToken.Revoke(ResourceMessages.RefreshTokenDefaultReason);

            // Assert
            refreshToken.IsActive.Should().BeFalse();
            refreshToken.RevokedReason.Should().Be(ResourceMessages.RefreshTokenDefaultReason);
            refreshToken.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Given_ValidRefreshToken_When_CheckIsValid_Then_ShouldReturnTrue()
        {
            // Arrange
            var refreshToken = new RefreshTokenBuilder().Build();

            // Act
            var isValid = refreshToken.IsValid;

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Given_ExpiredRefreshToken_When_CheckIsValid_Then_ShouldReturnFalse()
        {
            // Arrange
            var refreshToken = new RefreshTokenBuilder().BuildExpired();

            // Act
            var isValid = refreshToken.IsValid;

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void Given_RevokedRefreshToken_When_CheckIsValid_Then_ShouldReturnFalse()
        {
            // Arrange
            var refreshToken = new RefreshTokenBuilder().Build();
            refreshToken.Revoke(ResourceMessages.RefreshTokenDefaultReason);

            // Act
            var isValid = refreshToken.IsValid;

            // Assert
            isValid.Should().BeFalse();
        }
    }
}