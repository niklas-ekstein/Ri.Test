namespace Ri.Interview.Models;

public class TeamData
{
    public string Name { get; set; }
    public List<string> Admins { get; set; }
    public List<string> Members { get; set; }

    public TeamData()
    {
        Admins = new List<string>();
        Members = new List<string>();
    }
}