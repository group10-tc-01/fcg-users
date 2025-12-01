using FCG.Users.Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public static class AuthenticationServiceBuilder
    {
        private static readonly Mock<IAuthenticationService> _mock = new Mock<IAuthenticationService>();

        public static IAuthenticationService Build() => _mock.Object;

        public static void SetupValidateRefreshTokenAsync(string? userId)
        {
            _mock.Setup(service => service.ValidateRefreshTokenAsync(It.IsAny<string>())).ReturnsAsync(userId);
        }

        public static void SetupRevokeRefreshTokenAsync()
        {
            _mock.Setup(service => service.RevokeRefreshTokenAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        public static void SetupGenerateAccessToken(string accessToken)
        {
            _mock.Setup(service => service.GenerateAccessToken(It.IsAny<Domain.Users.User>())).Returns(accessToken);
        }

        public static void SetupGenerateRefreshToken(string refreshToken)
        {
            _mock.Setup(service => service.GenerateRefreshToken()).Returns(refreshToken);
        }

        public static void SetupCreateRefreshTokenAsync()
        {
            _mock.Setup(service => service.CreateRefreshTokenAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        }

        public static string GenerateToken(IConfiguration configuration, Guid userId, string role)
        {
            var jwtSettingsSection = configuration.GetSection("JwtSettings");

            var secretKey = jwtSettingsSection["SecretKey"];
            var issuer = jwtSettingsSection["Issuer"];
            var audience = jwtSettingsSection["Audience"];

            var expirationMinutes = configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(secretKey!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
