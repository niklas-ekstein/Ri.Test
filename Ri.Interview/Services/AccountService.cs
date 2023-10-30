using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ri.Interview;
using Ri.Interview.Interfaces;
using Ri.Interview.Models;
using Ri.Interview.Validations;

public class AccountService : IAccountService
{
    private readonly string _baseUrl;
    private static readonly HttpClient httpClient = new();

    public AccountService(IOptions<FormioSettings> settings)
    {
        _baseUrl = settings?.Value?.BaseUrl ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<bool> RegisterAsync(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));

        var endpoint = $"{_baseUrl}/user/register";

        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestData = new
        {
            data = new
            {
                name = account.Name,
                email = account.Email,
                password = account.Password
            }
        };

        var jsonString = JsonSerializer.Serialize(requestData);
        var httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(endpoint, httpContent);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                HandleBadRequest(await response.Content.ReadAsStringAsync());
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void HandleBadRequest(string responseBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var error = JsonSerializer.Deserialize<ValidationError>(responseBody, options);

        if (error?.Details?.Count > 0)
        {
            Console.WriteLine("Error Message: {ErrorMessage}", error.Details[0].Message);
        }
    }
}
