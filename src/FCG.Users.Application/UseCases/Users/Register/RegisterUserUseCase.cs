using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser != null)
            {
                throw new ConflictException(ResourceMessages.EmailAlreadyInUse);
            }

            var user = User.CreateRegularUser(request.Name, request.Email, request.Password);

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterUserResponse(user.Id);
        }
    }
}
