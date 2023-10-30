using System.Text.Json.Serialization;

namespace Ri.Interview.Models;
// Need to change the name of this class..
public class Settings
{
    [JsonPropertyName("cors")]
    public string? Cors { get; set; }
}