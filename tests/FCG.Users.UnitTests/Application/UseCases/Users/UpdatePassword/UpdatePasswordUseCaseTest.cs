using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Messages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.UnitTests.Application.UseCases.Users.UpdatePassword
{
    public class UpdatePasswordUseCaseTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypterService _passwordEncrypterService;
        private readonly ILoggedUserService _loggedUserService;
        private readonly Mock<ILogger<UpdatePasswordUseCase>> _logger;
        private readonly IUpdateUserUseCase _sut;

        public UpdatePasswordUseCaseTest()
        {
            _unitOfWork = UnitOfWorkBuilder.Build();
            _passwordEncrypterService = PasswordEncrypterServiceBuilder.Build();
            _loggedUserService = LoggedUserServiceBuilder.Build();
            _logger = new Mock<ILogger<UpdatePasswordUseCase>>();
            _sut = new UpdatePasswordUseCase(_unitOfWork, _passwordEncrypterService, _logger.Object, _loggedUserService);
        }

        [Fact]
        public async Task Given_ValidUpdatePasswordRequest_When_Handle_Then_ShouldUpdatePasswordSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new UpdatePasswordRequestBuilder().Build();

            LoggedUserServiceBuilder.SetupGetLoggedUserAsync(user);
            PasswordEncrypterServiceBuilder.SetupIsValid(true);
            PasswordEncrypterServiceBuilder.SetupEncrypt(request.NewPassword);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Given_InvalidCurrentPassword_When_Handle_Then_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new UpdatePasswordRequestBuilder().Build();

            LoggedUserServiceBuilder.SetupGetLoggedUserAsync(user);
            PasswordEncrypterServiceBuilder.SetupIsValid(false);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.CurrentPasswordIncorrect);
        }
    }
}
