using FCG.Users.Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public class AuthenticationServiceBuilder
    {
        private static readonly Mock<IAuthenticationService> _mock = new Mock<IAuthenticationService>();

        public static IAuthenticationService Build() => _mock.Object;

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
