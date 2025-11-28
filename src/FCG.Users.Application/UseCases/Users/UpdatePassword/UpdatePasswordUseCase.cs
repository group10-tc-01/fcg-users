using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users.ValueObjects;
using FCG.Users.Messages;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public class UpdatePasswordUseCase : IUpdateUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypterService _passwordEncrypterService;
        private readonly ILogger<UpdatePasswordUseCase> _logger;
        private readonly ILoggedUserService _loggedUser;

        public UpdatePasswordUseCase(
            IUnitOfWork unitOfWork,
            IPasswordEncrypterService passwordEncrypterService,
            ILogger<UpdatePasswordUseCase> logger,
            ILoggedUserService loggedUser)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypterService = passwordEncrypterService;
            _logger = logger;
            _loggedUser = loggedUser;
        }

        public async Task<UpdatePasswordResponse> Handle(UpdatePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _loggedUser.GetLoggedUserAsync();

            _logger.LogInformation("[UpdateUserUseCase] Updating password for user: {UserId}", user.Id);

            if (!_passwordEncrypterService.IsValid(request.CurrentPassword, user.Password.Value))
            {
                _logger.LogWarning("[UpdateUserUseCase] Invalid current password for user: {UserId}", user.Id);

                throw new DomainException(ResourceMessages.CurrentPasswordIncorrect);
            }

            Password newPassword = Password.Create(request.NewPassword);

            string hashedPassword = _passwordEncrypterService.Encrypt(newPassword.Value);

            _logger.LogInformation("[UpdateUserUseCase] Password updated for user: {UserId}", user.Id);

            user.Update(hashedPassword);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[UpdateUserUseCase] Successfully updated password for user: {UserId}", user.Id);

            return new UpdatePasswordResponse(user.Id, user.UpdatedAt);
        }
    }
}
