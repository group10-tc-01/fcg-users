using FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken;
using FCG.Users.Application.UseCases.Authentication.Login;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.Domain.Users;
using FCG.Users.IntegratedTests.Configurations;
using FCG.Users.Messages;
using FCG.Users.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace FCG.Users.IntegratedTests.Controllers
{
    public class AuthControllerTest : FcgFixture
    {
        public AuthControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        #region Login

        [Fact]
        public async Task Given_ValidLoginRequest_When_PostIsCalled_ShouldReturnOk()
        {
            // Arrange
            var validUrl = "/api/v1/auth/login";
            var createdUser = Factory.CreatedUsers.First();
            var loginRequest = new LoginRequestBuilder().BuildWithValues(createdUser.Email, createdUser.Password);
            Setup(createdUser);

            // Act
            var result = await DoPost(validUrl, loginRequest);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.AccessToken.Should().NotBeNullOrEmpty();
            apiResponse.Data.RefreshToken.Should().NotBeNullOrEmpty();
            apiResponse.Data.ExpiresInMinutes.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Given_InvalidLoginRequest_When_PostIsCalled_ShouldReturnBadRequest()
        {
            // Arrange
            var validUrl = "/api/v1/auth/login";
            var loginRequest = new LoginRequestBuilder().BuildWithValues("invalid_email", "passwordtest");

            // Act
            var result = await DoPost(validUrl, loginRequest);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeNull();
            apiResponse.ErrorMessages.Should().Contain(ResourceMessages.LoginInvalidEmailFormat);
        }

        #endregion

        #region RefreshToken

        [Fact]
        public async Task Given_ValidRefreshTokenRequest_When_PostIsCalled_ShouldReturnOk()
        {
            // Arrange
            var validUrl = "/api/v1/auth/refresh-token";
            var createdUser = Factory.CreatedUsers.First();
            var loginRequest = new LoginRequestBuilder().BuildWithValues(createdUser.Email, createdUser.Password);
            Setup(createdUser);
            var loginResult = await DoPost("/api/v1/auth/login", loginRequest);
            var loginResponseContent = await loginResult.Content.ReadAsStringAsync();
            var loginApiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(loginResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var jwtToken = loginApiResponse?.Data?.AccessToken!;

            Factory.CreatedRefreshTokens.Should().NotBeNull().And.NotBeEmpty();
            var createdRefreshToken = Factory.CreatedRefreshTokens.First();
            var refreshTokenInput = new RefreshTokenRequest(createdRefreshToken.Token);

            // Act
            Setup(createdUser);
            var result = await DoAuthenticatedPost(validUrl, refreshTokenInput, jwtToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<RefreshTokenResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.AccessToken.Should().NotBeNullOrEmpty();
            apiResponse.Data.RefreshToken.Should().NotBeNullOrEmpty();
            apiResponse.Data.ExpiresInDays.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Given_InvalidRefreshTokenRequest_When_PostIsCalled_ShouldReturnBadRequest()
        {
            // Arrange
            var validUrl = "/api/v1/auth/refresh-token";
            var createdUser = Factory.CreatedUsers.First();
            var loginRequest = new LoginRequestBuilder().BuildWithValues(createdUser.Email, createdUser.Password);
            Setup(createdUser);
            var loginResult = await DoPost("/api/v1/auth/login", loginRequest);
            var loginResponseContent = await loginResult.Content.ReadAsStringAsync();
            var loginApiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(loginResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var jwtToken = loginApiResponse?.Data.AccessToken!;
            var invalidRefreshToken = "invalid-refresh-token";
            var refreshTokenInput = new RefreshTokenRequest(invalidRefreshToken);

            // Act
            var result = await DoAuthenticatedPost(validUrl, refreshTokenInput, jwtToken);
            var responseContent = await result.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<RefreshTokenResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeFalse();
            apiResponse.Data.Should().BeNull();
            apiResponse.ErrorMessages.Should().Contain(ResourceMessages.InvalidRefreshToken);
        }

        private static void Setup(User user)
        {
            PasswordEncrypterServiceBuilder.Build();
            PasswordEncrypterServiceBuilder.SetupEncrypt(user.Password);
            PasswordEncrypterServiceBuilder.SetupIsValid(true);
        }

        #endregion
    }
}
