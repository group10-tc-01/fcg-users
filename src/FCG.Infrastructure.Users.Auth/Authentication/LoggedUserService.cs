using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Domain.Users;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

namespace FCG.Users.Infrastructure.Auth.Authentication
{
    [ExcludeFromCodeCoverage]
    public class LoggedUserService : ILoggedUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenProviderService _tokenProviderService;

        public LoggedUserService(IUserRepository userRepository, ITokenProviderService tokenProviderService)
        {
            _userRepository = userRepository;
            _tokenProviderService = tokenProviderService;
        }

        public async Task<User> GetLoggedUserAsync()
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(_tokenProviderService.GetToken());

            var userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")!.Value);

            var loggedUser = await _userRepository.GetByIdAsync(userId);

            return loggedUser!;
        }
    }
}
