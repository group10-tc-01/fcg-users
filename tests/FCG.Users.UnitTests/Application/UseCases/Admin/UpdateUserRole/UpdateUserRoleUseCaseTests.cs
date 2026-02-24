using FCG.Users.Application.UseCases.Admin.UpdateUserRole;
using FCG.Users.CommomTestsUtilities.Builders;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.Users;
using FluentAssertions;
using System.Net;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.UpdateUserRole
{
    public class UpdateUserRoleUseCaseTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUpdateUserRoleUseCase _sut;

        public UpdateUserRoleUseCaseTests()
        {
            _userRepository = UserRepositoryBuilder.Build();
            _unitOfWork = UnitOfWorkBuilder.Build();
            _sut = new UpdateUserRoleUseCase(_userRepository, _unitOfWork);
        }

        [Fact]
        public async Task Given_ValidUpdateUserRoleRequest_When_Handle_Then_ShouldUpdateUserRoleSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new UpdateUserRoleRequestBuilder().BuildWithCustomValues(user.Id, Role.Admin);
            UserRepositoryBuilder.SetupGetByIdAsync(user);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Given_UserNotFound_When_Handle_Then_ShouldReturnNotFoundFailure()
        {
            // Arrange
            var request = new UpdateUserRoleRequestBuilder().Build();
            UserRepositoryBuilder.SetupGetByIdAsync(null);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ErrorMessage.Should().Be("User not found.");
        }

        [Fact]
        public async Task Given_UpdateToAdminRole_When_Handle_Then_ShouldUpdateSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new UpdateUserRoleRequestBuilder().BuildWithCustomValues(user.Id, Role.Admin);
            UserRepositoryBuilder.SetupGetByIdAsync(user);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Given_UpdateToUserRole_When_Handle_Then_ShouldUpdateSuccessfully()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var request = new UpdateUserRoleRequestBuilder().BuildWithCustomValues(user.Id, Role.User);
            UserRepositoryBuilder.SetupGetByIdAsync(user);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Value!.Id.Should().Be(user.Id);
        }
    }
}
