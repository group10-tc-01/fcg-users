using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Users;
using FCG.Users.Infrastructure.Auth.Authentication;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FCG.Users.UnitTests.Auth
{
    public class LoggedUserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenProviderService> _tokenProviderServiceMock;
        private readonly LoggedUserService _sut;

        public LoggedUserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenProviderServiceMock = new Mock<ITokenProviderService>();
            _sut = new LoggedUserService(_userRepositoryMock.Object, _tokenProviderServiceMock.Object);
        }

        [Fact]
        public async Task Given_ValidToken_When_GetLoggedUserAsync_Then_ShouldReturnUser()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var token = GenerateValidToken(user.Id);

            _tokenProviderServiceMock.Setup(x => x.GetToken()).Returns(token);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _sut.GetLoggedUserAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task Given_TokenWithUserId_When_GetLoggedUserAsync_Then_ShouldCallRepositoryWithCorrectId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().Build();
            var token = GenerateValidToken(userId);

            _tokenProviderServiceMock.Setup(x => x.GetToken()).Returns(token);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _sut.GetLoggedUserAsync();

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        private static string GenerateValidToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
