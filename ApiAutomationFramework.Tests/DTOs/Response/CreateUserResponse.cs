using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class CreateUserResponse
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("job")]
    public string Job { get; set; } = string.Empty;

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}