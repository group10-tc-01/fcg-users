using FCG.Users.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace FCG.Users.Infrastructure.Auth.Authentication
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetToken()
        {
            var authentication = _httpContextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

            return authentication["Bearer ".Length..].Trim();
        }
    }
}
