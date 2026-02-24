using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.Abstractions.Results;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using System.Net;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypterService _passwordEncrypter;

        public RegisterUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordEncrypterService passwordEncrypter)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
        }

        public async Task<Result<RegisterUserResponse>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<RegisterUserResponse>.Failure(ResourceMessages.EmailAlreadyInUse, HttpStatusCode.Conflict);
            }

            var hashedPassword = _passwordEncrypter.Encrypt(request.Password);

            var user = User.CreateRegularUser(request.Name, request.Email, hashedPassword);

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<RegisterUserResponse>.Success(new RegisterUserResponse(user.Id));
        }
    }
}
