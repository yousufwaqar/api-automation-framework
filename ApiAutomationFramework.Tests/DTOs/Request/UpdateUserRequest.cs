using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Request;

public class UpdateUserRequest
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("job")]
    public string? Job { get; set; }

    public static UpdateUserRequest Default() => new()
    {
        Name = "Jane Doe Updated",
        Job = "Senior Engineer"
    };
}