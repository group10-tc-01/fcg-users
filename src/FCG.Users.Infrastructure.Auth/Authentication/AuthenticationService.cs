using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Settings;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.RefreshTokens;
using FCG.Users.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FCG.Users.Infrastructure.Auth.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(IOptions<JwtSettings> jwtSettings, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email.Value),
                    new(ClaimTypes.GivenName, user.Name.Value),
                    new(ClaimTypes.Role, user.Role.ToString())
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task CreateRefreshTokenAsync(string token, Guid userId)
        {
            var refreshToken = RefreshToken.Create(token, userId, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token?.IsValid == true)
                return token.UserId.ToString();

            return null;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token != null)
            {
                token.Revoke();
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
