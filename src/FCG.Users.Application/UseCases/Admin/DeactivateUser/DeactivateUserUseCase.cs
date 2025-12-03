using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.UseCases.Admin.DeactivateUser
{
    public class DeactivateUserUseCase : IDeactivateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateUserUseCase> _logger;

        public DeactivateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeactivateUserUseCase> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DeactivateUserResponse> Handle(DeactivateUserRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[DeactivateUserUseCase] Attempting to deactivate user: {UserId}", request.Id);

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("[DeactivateUserUseCase] User not found: {UserId}", request.Id);
                throw new NotFoundException(ResourceMessages.UserNotFound);
            }

            user.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[DeactivateUserUseCase] Successfully deactivated user: {UserId}, IsActive: {IsActive}", user.Id, user.IsActive);

            return new DeactivateUserResponse(user.Id, user.IsActive);
        }
    }
}
