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

        public UserControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Given_ValidRequest_When_PostIsCalled_ShouldReturnCreated()
        {
            // Arrange
            var request = new RegisterUserRequestBuilder().Build();

            // Act
            var result = await DoPost(RegisterUrl, request);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Guid>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            apiResponse.Should().NotBeNull();
        }
    }
}
