using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FCG.Users.IntegratedTests.Configurations
{
    public class FcgFixture : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _httpClient;
        protected readonly CustomWebApplicationFactory Factory;
        private readonly IConfiguration _configuration;

        public FcgFixture(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            _httpClient = factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        protected string GenerateToken(Guid userId, string role)
        {
            return AuthenticationServiceBuilder.GenerateToken(_configuration, userId, role);
        }

        protected async Task<HttpResponseMessage> DoPost<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedPost<T>(string url, T content, string jwtToken)
        {
            SetAuthenticationHeader(jwtToken);
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedPatch<T>(string url, T content, string jwtToken)
        {
            SetAuthenticationHeader(jwtToken);
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = stringContent
            };
            return await _httpClient.SendAsync(request);
        }

        private void SetAuthenticationHeader(string jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }
    }
}