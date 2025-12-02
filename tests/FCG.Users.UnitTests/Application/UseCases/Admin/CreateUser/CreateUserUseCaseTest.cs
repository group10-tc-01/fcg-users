using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.UseCases.Admin.CreateUser;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.CreateUser
{
    public class CreateUserUseCaseTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypterService _passwordEncrypter;
        private readonly ICreateUserUseCase _sut;

        public CreateUserUseCaseTest()
        {
            _userRepository = UserRepositoryBuilder.Build();
            _unitOfWork = UnitOfWorkBuilder.Build();
            _passwordEncrypter = PasswordEncrypterServiceBuilder.Build();
            _sut = new CreateUserUseCase(_userRepository, _unitOfWork, _passwordEncrypter);
        }

        [Fact]
        public async Task Given_ValidCreateUserRequest_When_Handle_Then_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var request = new CreateUserRequestBuilder().Build();
            UserRepositoryBuilder.SetupGetByEmailAsync(null);
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
            var request = new CreateUserRequestBuilder().Build();
            var user = new UserBuilder().Build();
            UserRepositoryBuilder.SetupGetByEmailAsync(user);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>().WithMessage(ResourceMessages.EmailAlreadyInUse);
        }

        [Fact]
        public async Task Given_AdminRole_When_Handle_Then_ShouldCreateAdminUser()
        {
            // Arrange
            var request = new CreateUserRequestBuilder().BuildWithCustomValues("Admin User", "admin@test.com", "Password@123", Role.Admin);
            UserRepositoryBuilder.SetupGetByEmailAsync(null);
            PasswordEncrypterServiceBuilder.SetupEncrypt(request.Password);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Given_UserRole_When_Handle_Then_ShouldCreateRegularUser()
        {
            // Arrange
            var request = new CreateUserRequestBuilder().BuildWithCustomValues("Regular User", "user@test.com", "Password@123", Role.User);
            UserRepositoryBuilder.SetupGetByEmailAsync(null);
            PasswordEncrypterServiceBuilder.SetupEncrypt(request.Password);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
        }
    }
}
