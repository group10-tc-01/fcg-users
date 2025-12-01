using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Users.Register
{
    public class RegisterUserUseCaseTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypterService _passwordEncrypter;
        private readonly IRegisterUserUseCase _sut;

        public RegisterUserUseCaseTest()
        {
            _userRepository = UserRepositoryBuilder.Build();
            _unitOfWork = UnitOfWorkBuilder.Build();
            _passwordEncrypter = PasswordEncrypterServiceBuilder.Build();
            _sut = new RegisterUserUseCase(_userRepository, _unitOfWork, _passwordEncrypter);
        }


        [Fact]
        public async Task Given_ValidRegisterRequest_When_Handle_Then_ShouldRegisterUserSuccessfully()
        {
            // Arrange
            var request = new RegisterUserRequestBuilder().Build();
            PasswordEncrypterServiceBuilder.SetupEncrypt(request.Password);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Given_EmailAlreadyExists_When_Handle_Then_ShouldThrowConflictException()
        {
            // Arrange
            var request = new RegisterUserRequestBuilder().Build();
            var user = new UserBuilder().Build();
            UserRepositoryBuilder.SetupGetByEmailAsync(user);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>().WithMessage(ResourceMessages.EmailAlreadyInUse);
        }

    }
}
