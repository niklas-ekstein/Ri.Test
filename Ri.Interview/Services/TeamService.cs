using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ri.Interview;
using Ri.Interview.Models;
using Ri.Interview.Interfaces;
using Ri.Interview.Validations;

public class TeamService : ITeamService
{
    private readonly string _portalBaseUrl;
    private static readonly HttpClient httpClient = new();

    public TeamService(IOptions<FormioSettings> settings)
    {
        _portalBaseUrl = settings?.Value?.BaseUrl ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<bool> CreateTeamAsync(string jwtToken, Team team)
    {
        if (team == null) throw new ArgumentNullException(nameof(team));

        var endpoint = $"{_portalBaseUrl}/team/submission";

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-jwt-token", jwtToken);

        var jsonString = JsonSerializer.Serialize(team);
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
