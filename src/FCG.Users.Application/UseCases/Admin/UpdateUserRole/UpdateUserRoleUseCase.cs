using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public class UpdateUserRoleUseCase : IUpdateUserRoleUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserRoleUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateUserRoleResponse> Handle(UpdateUserRoleRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.UpdateRole(request.NewRole);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateUserRoleResponse(user.Id);
        }
    }
}
