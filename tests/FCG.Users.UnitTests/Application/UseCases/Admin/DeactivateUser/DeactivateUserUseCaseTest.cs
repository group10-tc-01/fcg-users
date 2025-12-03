using FCG.Users.Application.UseCases.Admin.DeactivateUser;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users;
using FCG.Users.Messages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.DeactivateUser
{
    public class DeactivateUserUseCaseTest
    {
        private readonly Mock<ILogger<DeactivateUserUseCase>> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeactivateUserUseCase _sut;

        public DeactivateUserUseCaseTest()
        {
            _userRepository = UserRepositoryBuilder.Build();
            _unitOfWork = UnitOfWorkBuilder.Build();
            _logger = new Mock<ILogger<DeactivateUserUseCase>>();
            _sut = new DeactivateUserUseCase(_userRepository, _unitOfWork, _logger?.Object);
        }

        [Fact]
        public async Task Given_ValidDeactivateUserRequest_When_Handle_Then_ShouldDeactivateUserSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new DeactivateUserRequestBuilder().BuildWithId(user.Id);

            UserRepositoryBuilder.SetupGetByIdAsync(user);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(user.Id);
            response.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Given_UserNotFound_When_Handle_Then_ShouldThrowNotFoundException()
        {
            // Arrange
            var request = new DeactivateUserRequestBuilder().Build();

            UserRepositoryBuilder.SetupGetByIdAsync(null);

            // Act
            var act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage(ResourceMessages.UserNotFound);
        }

        [Fact]
        public async Task Given_ActiveUser_When_Handle_Then_ShouldChangeIsActiveToFalse()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new DeactivateUserRequestBuilder().BuildWithId(user.Id);

            UserRepositoryBuilder.SetupGetByIdAsync(user);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.IsActive.Should().BeFalse();
            user.IsActive.Should().BeFalse();
        }
    }
}