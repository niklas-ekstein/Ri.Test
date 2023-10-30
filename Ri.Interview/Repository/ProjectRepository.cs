using Ri.Interview.Models;
using Microsoft.EntityFrameworkCore;

namespace Ri.Interview.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Project> GetAll()
        {
            return _context.Projects.ToList();
        }

        public Project GetById(int id)
        {
            return _context.Projects.Find(id);
        }

        public void Add(Project project)
        {
            _context.Projects.Add(project);
        }

        public void Update(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var project = GetById(id);
            _context.Projects.Remove(project);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}