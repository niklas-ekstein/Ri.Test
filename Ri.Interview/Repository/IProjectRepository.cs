using Ri.Interview.Models;

namespace Ri.Interview
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAll();
        Project GetById(int id);
        void Add(Project project);
        void Update(Project project);
        void Delete(int id);
        void SaveChanges();
    }
}