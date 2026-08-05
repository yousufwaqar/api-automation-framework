using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class UserData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("avatar")]
    public string Avatar { get; set; } = string.Empty;

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}
