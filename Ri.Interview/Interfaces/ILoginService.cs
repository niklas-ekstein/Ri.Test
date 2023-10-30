namespace Ri.Interview.Interfaces;

public interface ILoginService
{
    Task<(bool Success, string Token)> LoginAsync(string email, string password);
}