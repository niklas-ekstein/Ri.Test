using Ri.Interview.Models;

namespace Ri.Interview.Interfaces;

public interface IProjectService
{
    Task<bool> CreateProjectAsync(string jwtToken, Project project);
    Task<bool> UpdateProjectAsync(string jwtToken, string projectId, Project updatedProject);
    Task<IEnumerable<Project>> GetAllProjectsAsync(string jwtToken);
}