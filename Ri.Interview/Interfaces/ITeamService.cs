using System.Threading.Tasks;
using Ri.Interview.Models;

namespace Ri.Interview.Interfaces
{
    public interface ITeamService
    {
        Task<bool> CreateTeamAsync(string jwtToken, Team team);
    }
}