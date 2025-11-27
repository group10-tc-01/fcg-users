using System.Text;
using System.Text.Json;

namespace FCG.Users.IntegratedTests.Configurations
{
    public class FcgFixture : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _httpClient;
        protected readonly CustomWebApplicationFactory Factory;

        public FcgFixture(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            _httpClient = factory.CreateClient();
        }

        protected async Task<HttpResponseMessage> DoPost<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, stringContent);
        }
    }
}