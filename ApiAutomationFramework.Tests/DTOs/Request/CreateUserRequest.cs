using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Request;

public class CreateUserRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("job")]
    public string Job { get; set; } = string.Empty;

    public static CreateUserRequest Default() => new()
    {
        Name = "John Doe",
        Job = "Software Engineer"
    };
}