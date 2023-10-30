using System.Text.Json.Serialization;

namespace Ri.Interview.Models;

public class Project
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("template")] 
    public string? Template { get; set; }
    
    [JsonPropertyName("settings")]
    public Settings? Settings { get; set; }
}