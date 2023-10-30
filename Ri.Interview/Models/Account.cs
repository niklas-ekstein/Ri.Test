namespace Ri.Interview.Models;

public class Account
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public Account(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}