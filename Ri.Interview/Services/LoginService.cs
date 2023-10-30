using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ri.Interview.Interfaces;

namespace Ri.Interview.Services
{
    public class LoginService : ILoginService
    {
        private readonly string _baseUrl;
        private static readonly HttpClient httpClient = new();

        public LoginService(IOptions<FormioSettings> settings)
        {
            _baseUrl = settings.Value.BaseUrl;
        }

        public async Task<(bool Success, string Token)> LoginAsync(string email, string password)
        {
            var endpoint = $"{_baseUrl}/user/login";

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginData = new
            {
                data = new
                {
                    email = email,
                    password = password
                }
            };

            var jsonString = JsonSerializer.Serialize(loginData);
            var httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(endpoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                
                if (response.Headers.TryGetValues("x-jwt-token", out var tokenValues))
                {
                    var jwtToken = tokenValues.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(jwtToken))
                    {
                        return (true, jwtToken);
                    }
                }
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenObject = JsonSerializer.Deserialize<TokenResponse>(responseBody);
                return (true, tokenObject?.Token);
            }

            return (false, null);
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}