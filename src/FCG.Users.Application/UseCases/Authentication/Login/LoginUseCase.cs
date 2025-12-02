using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Settings;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IPasswordEncrypterService _passwordEncrypter;
        private readonly ILogger<LoginUseCase> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public LoginUseCase(
            IUserRepository userRepository,
            IAuthenticationService authenticationService,
            ILogger<LoginUseCase> logger,
            IPasswordEncrypterService passwordEncrypter,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _logger = logger;
            _passwordEncrypter = passwordEncrypter;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[LoginUseCase] Starting login process for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            ValidateUser(request, user);

            var accessToken = _authenticationService.GenerateAccessToken(user!);
            var refreshToken = _authenticationService.GenerateRefreshToken();
            await _authenticationService.CreateRefreshTokenAsync(refreshToken, user!.Id);

            _logger.LogInformation("[LoginUseCase] Login successful for user: {UserId}", user!.Id);

            return new LoginResponse(accessToken, refreshToken, _jwtSettings.AccessTokenExpirationMinutes);
        }

        private void ValidateUser(LoginRequest request, User? user)
        {
            if (user is null)
                throw new UnauthorizedException(ResourceMessages.InvalidEmailOrPassword);

            var isPasswordValid = _passwordEncrypter.IsValid(request.Password, user.Password.Value);

            if (isPasswordValid is false)
                throw new UnauthorizedException(ResourceMessages.InvalidEmailOrPassword);
        }
    }
}
