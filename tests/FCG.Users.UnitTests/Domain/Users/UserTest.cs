using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Users;
using FCG.Users.Domain.Users.Events;
using FluentAssertions;

namespace FCG.Users.UnitTests.Domain.Users
{
    public class UserTest
    {
        [Fact]
        public void Given_ValidUserParameters_When_Create_Then_ShouldIntantiateUser()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();

            // Act
            var user = User.CreateRegularUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBe(Guid.Empty);
            user.Name.Should().Be(userBuilder.Name);
            user.Email.Should().Be(userBuilder.Email);
            user.Role.Should().Be(Role.User);
            user.RefreshTokens.Should().BeNullOrEmpty();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            user.UpdatedAt.TimeOfDay.Should().Be(TimeSpan.Zero);
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Given_ValidUserParameters_When_Create_Then_ShouldIntantiateAdminUser()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();

            // Act
            var user = User.CreateAdminUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBe(Guid.Empty);
            user.Name.Should().Be(userBuilder.Name);
            user.Email.Should().Be(userBuilder.Email);
            user.Role.Should().Be(Role.Admin);
            user.RefreshTokens.Should().BeNullOrEmpty();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            user.UpdatedAt.TimeOfDay.Should().Be(TimeSpan.Zero);
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Given_Deactivate_Called_When_UserIsActive_Then_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var user = new UserBuilder().Build();

            // Act
            user.Deactivate();

            // Assert
            user.IsActive.Should().BeFalse();
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void Given_Activate_Called_When_UserIsInactive_Then_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var user = new UserBuilder().Build();
            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            user.IsActive.Should().BeTrue();
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void Given_UserCreated_When_GetDomainEvents_Then_ShouldReturnUserCreatedDomainEvent()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();

            // Act
            var user = User.CreateRegularUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);
            var domainEvents = user.GetDomainEvents();

            // Assert
            domainEvents.Should().NotBeNull();
            domainEvents.Should().HaveCount(1);
            domainEvents.First().Should().BeOfType<UserCreatedDomainEvent>();

            var userCreatedEvent = domainEvents.First() as UserCreatedDomainEvent;
            userCreatedEvent!.UserId.Should().Be(user.Id);
            userCreatedEvent.Name.Should().Be(userBuilder.Name);
            userCreatedEvent.Email.Should().Be(userBuilder.Email);
        }

        [Fact]
        public void Given_UserWithDomainEvents_When_ClearDomainEvents_Then_ShouldRemoveAllEvents()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();
            var user = User.CreateRegularUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);

            user.GetDomainEvents().Should().HaveCount(1);

            // Act
            user.ClearDomainEvents();

            // Assert
            var domainEvents = user.GetDomainEvents();
            domainEvents.Should().NotBeNull();
            domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void Given_UserWithNoDomainEvents_When_GetDomainEvents_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();
            var user = User.CreateRegularUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);
            user.ClearDomainEvents();

            // Act
            var domainEvents = user.GetDomainEvents();

            // Assert
            domainEvents.Should().NotBeNull();
            domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void Given_ValidUserParameters_When_Update_Then_ShouldReturnOk()
        {
            // Arrange
            var userBuilder = new UserBuilder().Build();
            var newPassword = "ValidP@ssw0rd123!";

            // Act
            var user = User.CreateRegularUser(userBuilder.Name, userBuilder.Email, userBuilder.Password);
            user.UpdatePassword(newPassword);

            // Assert
            user.Password.Value.Should().Be(newPassword);
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }
    }
}
