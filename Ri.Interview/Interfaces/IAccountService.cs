using Ri.Interview.Models;

namespace Ri.Interview.Interfaces;

public interface IAccountService
{
    Task<bool> RegisterAsync(Account account);
}