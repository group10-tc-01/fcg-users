using FCG.Users.Application.UseCases.Admin.GetUsers;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.GetUsers
{
    public class GetUsersUseCaseTests
    {
        private readonly Mock<ILogger<GetUsersUseCase>> _logger;
        private readonly IGetUsersUseCase _sut;

        public GetUsersUseCaseTests()
        {
            var userRepository = UserRepositoryBuilder.Build();
            _logger = new Mock<ILogger<GetUsersUseCase>>();
            _sut = new GetUsersUseCase(userRepository, _logger.Object);
        }

        [Fact]
        public async Task Given_ValidGetUsersRequest_When_Handle_Then_ShouldReturnPagedUsersSuccessfully()
        {
            // Arrange
            var users = new List<User>
            {
                new UserBuilder().Build(),
                new UserBuilder().Build(),
                new UserBuilder().Build()
            };
            var totalCount = 3;
            var request = new GetUsersRequestBuilder().Build();

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Items.Should().HaveCount(3);
            response.TotalCount.Should().Be(totalCount);
            response.CurrentPage.Should().Be(request.PageNumber);
            response.PageSize.Should().Be(request.PageSize);
        }

        [Fact]
        public async Task Given_GetUsersRequestWithFilters_When_Handle_Then_ShouldReturnFilteredUsers()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var users = new List<User> { user };
            var totalCount = 1;
            var request = new GetUsersRequestBuilder().BuildWithFilters("John", "john@example.com");

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Items.Should().HaveCount(1);
            response.TotalCount.Should().Be(totalCount);
        }

        [Fact]
        public async Task Given_GetUsersRequestWithNoResults_When_Handle_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var users = new List<User>();
            var totalCount = 0;
            var request = new GetUsersRequestBuilder().Build();

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Items.Should().BeEmpty();
            response.TotalCount.Should().Be(0);
            response.TotalPages.Should().Be(0);
        }

        [Fact]
        public async Task Given_GetUsersRequestWithPagination_When_Handle_Then_ShouldReturnCorrectPageMetadata()
        {
            // Arrange
            var users = new List<User>
            {
                new UserBuilder().Build(),
                new UserBuilder().Build()
            };
            var totalCount = 25;
            var request = new GetUsersRequestBuilder().BuildWithCustomValues(pageNumber: 2, pageSize: 10);

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.CurrentPage.Should().Be(2);
            response.PageSize.Should().Be(10);
            response.TotalPages.Should().Be(3);
            response.HasPrevious.Should().BeTrue();
            response.HasNext.Should().BeTrue();
        }

        [Fact]
        public async Task Given_GetUsersRequest_When_Handle_Then_ShouldMapUserPropertiesCorrectly()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var users = new List<User> { user };
            var totalCount = 1;
            var request = new GetUsersRequestBuilder().Build();

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.Items.Should().HaveCount(1);
            var userResponse = response.Items.First();
            userResponse.Id.Should().Be(user.Id);
            userResponse.Name.Should().Be(user.Name.Value);
            userResponse.Email.Should().Be(user.Email.Value);
            userResponse.Role.Should().Be(user.Role.ToString());
            userResponse.CreatedAt.Should().Be(user.CreatedAt);
        }

        [Fact]
        public async Task Given_GetUsersRequestOnLastPage_When_Handle_Then_ShouldIndicateNoNextPage()
        {
            // Arrange
            var users = new List<User>
            {
                new UserBuilder().Build(),
                new UserBuilder().Build()
            };
            var totalCount = 12;
            var request = new GetUsersRequestBuilder().BuildWithCustomValues(pageNumber: 2, pageSize: 10);

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.HasNext.Should().BeFalse();
            response.HasPrevious.Should().BeTrue();
        }

        [Fact]
        public async Task Given_GetUsersRequestOnFirstPage_When_Handle_Then_ShouldIndicateNoPreviousPage()
        {
            // Arrange
            var users = new List<User>
            {
                new UserBuilder().Build()
            };
            var totalCount = 1;
            var request = new GetUsersRequestBuilder().BuildWithCustomValues(pageNumber: 1, pageSize: 10);

            UserRepositoryBuilder.SetupGetPagedUsersAsync(users, totalCount);

            // Act
            var response = await _sut.Handle(request, CancellationToken.None);

            // Assert
            response.HasPrevious.Should().BeFalse();
            response.HasNext.Should().BeFalse();
        }
    }
}
