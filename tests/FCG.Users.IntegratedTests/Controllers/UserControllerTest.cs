using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.IntegratedTests.Configurations;
using FCG.Users.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace FCG.Users.IntegratedTests.Controllers
{
    public class UserControllerTest : FcgFixture
    {
        private const string RegisterUrl = "/api/v1/users";
        private const string UpdatePasswordUrl = "/api/v1/users/update-password";

        public UserControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Given_ValidRequest_When_PostIsCalled_ShouldReturnCreated()
        {
            // Arrange
            var request = new RegisterUserRequestBuilder().Build();
            PasswordEncrypterServiceBuilder.SetupEncrypt();

            // Act
            var result = await DoPost(RegisterUrl, request);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<RegisterUserResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            apiResponse.Should().NotBeNull();
            apiResponse.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Given_UpdatePasswordRequest_When_PatchIsCalled_ShouldReturnOk()
        {
            // Arrange
            var user = Factory.CreatedUsers.First();
            var request = new UpdatePasswordRequestBuilder().Build();
            var userToken = GenerateToken(user.Id, user.Role.ToString());
            PasswordEncrypterServiceBuilder.SetupIsValid(true);
            PasswordEncrypterServiceBuilder.SetupEncrypt();

            // Act
            var response = await DoAuthenticatedPatch(UpdatePasswordUrl, request, userToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<UpdatePasswordResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Data.Id.Should().NotBeEmpty();
            apiResponse.Data.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }
    }
}
