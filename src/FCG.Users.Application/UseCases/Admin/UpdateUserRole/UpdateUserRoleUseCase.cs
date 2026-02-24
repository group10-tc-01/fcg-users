using FCG.Users.Application.Abstractions.Results;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Users;
using System.Net;

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

        public async Task<Result<UpdateUserRoleResponse>> Handle(UpdateUserRoleRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return Result<UpdateUserRoleResponse>.Failure("User not found.", HttpStatusCode.NotFound);
            }

            user.UpdateRole(request.NewRole);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UpdateUserRoleResponse>.Success(new UpdateUserRoleResponse(user.Id));
        }
    }
}
