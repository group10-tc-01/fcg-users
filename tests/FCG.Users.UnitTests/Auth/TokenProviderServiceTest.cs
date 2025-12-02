using FCG.Users.Infrastructure.Auth.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FCG.Users.UnitTests.Auth
{
    public class TokenProviderServiceTest
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly TokenProviderService _sut;

        public TokenProviderServiceTest()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _sut = new TokenProviderService(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void Given_ValidBearerToken_When_GetToken_Then_ShouldReturnToken()
        {
            // Arrange
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ";
            var authorizationHeader = $"Bearer {expectedToken}";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = authorizationHeader;
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var token = _sut.GetToken();

            // Assert
            token.Should().Be(expectedToken);
        }

        [Fact]
        public void Given_BearerTokenWithExtraSpaces_When_GetToken_Then_ShouldReturnTrimmedToken()
        {
            // Arrange
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
            var authorizationHeader = $"Bearer   {expectedToken}   ";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = authorizationHeader;
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var token = _sut.GetToken();

            // Assert
            token.Should().Be(expectedToken);
        }

        [Fact]
        public void Given_EmptyAuthorizationHeader_When_GetToken_Then_ShouldReturnEmptyString()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer ";
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var token = _sut.GetToken();

            // Assert
            token.Should().BeEmpty();
        }
    }
}
