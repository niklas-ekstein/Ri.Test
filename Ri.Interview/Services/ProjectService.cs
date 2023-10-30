using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ri.Interview.Interfaces;
using Ri.Interview.Models;
using Ri.Interview.Validations;

namespace Ri.Interview.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly string _apiUrl;
        private static readonly HttpClient httpClient = new();

        public ProjectService(IOptions<FormioSettings> settings, IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _apiUrl = settings?.Value?.ApiUrl ?? throw new ArgumentNullException(nameof(settings));
        }

        private void SetHttpClientHeaders(string jwtToken)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-jwt-token", jwtToken);
        }

        public async Task<bool> CreateProjectAsync(string jwtToken, Project project)
        {
            if (string.IsNullOrWhiteSpace(jwtToken)) throw new ArgumentNullException(nameof(jwtToken));
            if (project == null) throw new ArgumentNullException(nameof(project));

            var endpoint = $"{_apiUrl}/project";

            SetHttpClientHeaders(jwtToken);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var jsonString = JsonSerializer.Serialize(project, options);

            using var httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(endpoint, httpContent);
                HandleErrors(response);

                if (!response.IsSuccessStatusCode) return false;

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiProject = JsonSerializer.Deserialize<Project>(responseBody, deserializeOptions);

                if (apiProject != null)
                {
                    project.Id = apiProject.Id;
                }

                _projectRepository.Add(project);
                _projectRepository.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProjectAsync(string jwtToken, string projectId, Project updatedProject)
        {
            if (string.IsNullOrWhiteSpace(jwtToken)) throw new ArgumentNullException(nameof(jwtToken));
            if (string.IsNullOrWhiteSpace(projectId)) throw new ArgumentNullException(nameof(projectId));
            if (updatedProject == null) throw new ArgumentNullException(nameof(updatedProject));

            var endpoint = $"{_apiUrl}/project/{projectId}";

            SetHttpClientHeaders(jwtToken);

            var jsonString = JsonSerializer.Serialize(updatedProject);
            var httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PutAsync(endpoint, httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                HandleErrors(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken)) throw new ArgumentNullException(nameof(jwtToken));

            var endpoint = $"{_apiUrl}/project";

            SetHttpClientHeaders(jwtToken);

            var dbProjects = _projectRepository.GetAll();
            if (dbProjects.Any())
            {
                return dbProjects;
            }

            try
            {
                var response = await httpClient.GetAsync(endpoint);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                HandleErrors(response);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var projects = JsonSerializer.Deserialize<IEnumerable<Project>>(responseBody, options);

                foreach (var project in projects)
                {
                    project.Id = project.Title;
                    _projectRepository.Add(project);
                }

                _projectRepository.SaveChanges();

                return projects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Console.WriteLine($"Inner exception: {innerEx.Message}");
                    innerEx = innerEx.InnerException;
                }

                return null;
            }
        }
        
        private void HandleErrors(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
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
    }
}
