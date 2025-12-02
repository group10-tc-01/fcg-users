using FCG.Users.Application.Abstractions.Pagination;
using FCG.Users.Application.UseCases.Admin.CreateUser;
using FCG.Users.Application.UseCases.Admin.GetUsers;
using FCG.Users.Domain.Users;
using FCG.Users.IntegratedTests.Configurations;
using FCG.Users.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace FCG.Users.IntegratedTests.Controllers
{
    public class AdminControllerTest : FcgFixture
    {
        private const string GetUsersUrl = "/api/v1/admin/users";
        private const string CreateUserUrl = "/api/v1/admin/users";

        public AdminControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Given_AdminUser_When_GetUsersIsCalled_ShouldReturnOkWithPagedUsers()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());

            // Act
            var result = await DoAuthenticatedGet(GetUsersUrl, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedListResponse<GetUsersResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Items.Should().NotBeEmpty();
            apiResponse.Data.TotalCount.Should().BeGreaterThan(0);
            apiResponse.Data.CurrentPage.Should().Be(1);
            apiResponse.Data.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task Given_AdminUser_When_GetUsersWithFiltersIsCalled_ShouldReturnFilteredUsers()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var firstUser = Factory.CreatedUsers.First();
            var url = $"{GetUsersUrl}?Email={firstUser.Email.Value}";

            // Act
            var result = await DoAuthenticatedGet(url, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedListResponse<GetUsersResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Items.Should().HaveCount(1);
            apiResponse.Data.Items.First().Email.Should().Be(firstUser.Email.Value);
        }

        [Fact]
        public async Task Given_AdminUser_When_GetUsersWithPaginationIsCalled_ShouldReturnCorrectPage()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var url = $"{GetUsersUrl}?PageNumber=1&PageSize=1";

            // Act
            var result = await DoAuthenticatedGet(url, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedListResponse<GetUsersResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Data.Items.Should().HaveCount(1);
            apiResponse.Data.PageSize.Should().Be(1);
            apiResponse.Data.HasNext.Should().BeTrue();
        }

        [Fact]
        public async Task Given_RegularUser_When_GetUsersIsCalled_ShouldReturnForbidden()
        {
            // Arrange
            var regularUser = Factory.CreatedUsers.First();
            var userToken = GenerateToken(regularUser.Id, regularUser.Role.ToString());

            // Act
            var result = await DoAuthenticatedGet(GetUsersUrl, userToken);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Given_NoAuthentication_When_GetUsersIsCalled_ShouldReturnUnauthorized()
        {
            // Arrange & Act
            var result = await _httpClient.GetAsync(GetUsersUrl);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Given_EmptyDatabase_When_GetUsersIsCalled_ShouldReturnEmptyList()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var url = $"{GetUsersUrl}?Email=nonexistent@example.com";

            // Act
            var result = await DoAuthenticatedGet(url, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedListResponse<GetUsersResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Data.Items.Should().BeEmpty();
            apiResponse.Data.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Given_AdminUser_When_CreateUserIsCalled_ShouldReturnCreatedWithUserId()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var request = new CreateUserRequest("New Test User", "newuser@test.com", "Password@123", Role.User);

            // Act
            var result = await DoAuthenticatedPost(CreateUserUrl, request, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateUserResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Given_AdminUser_When_CreateAdminUserIsCalled_ShouldReturnCreatedWithUserId()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var request = new CreateUserRequest("New Admin User", "newadmin@test.com", "Password@123", Role.Admin);

            // Act
            var result = await DoAuthenticatedPost(CreateUserUrl, request, adminToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateUserResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Given_AdminUser_When_CreateUserWithExistingEmailIsCalled_ShouldReturnConflict()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var existingUser = Factory.CreatedUsers.First();
            var request = new CreateUserRequest("Duplicate User", existingUser.Email.Value, "Password@123", Role.User);

            // Act
            var result = await DoAuthenticatedPost(CreateUserUrl, request, adminToken);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Given_AdminUser_When_CreateUserWithInvalidDataIsCalled_ShouldReturnBadRequest()
        {
            // Arrange
            var adminUser = Factory.CreatedAdminUser;
            var adminToken = GenerateToken(adminUser.Id, adminUser.Role.ToString());
            var request = new CreateUserRequest("", "invalid-email", "short", Role.User);

            // Act
            var result = await DoAuthenticatedPost(CreateUserUrl, request, adminToken);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Given_RegularUser_When_CreateUserIsCalled_ShouldReturnForbidden()
        {
            // Arrange
            var regularUser = Factory.CreatedUsers.First();
            var userToken = GenerateToken(regularUser.Id, regularUser.Role.ToString());
            var request = new CreateUserRequest("Test User", "test@test.com", "Password@123", Role.User);

            // Act
            var result = await DoAuthenticatedPost(CreateUserUrl, request, userToken);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Given_NoAuthentication_When_CreateUserIsCalled_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new CreateUserRequest("Test User", "test@test.com", "Password@123", Role.User);

            // Act
            var result = await DoPost(CreateUserUrl, request);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
