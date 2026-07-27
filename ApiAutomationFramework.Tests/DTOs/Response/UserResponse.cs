using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class UserResponse
{
    [JsonProperty("data")]
    public UserData? Data { get; set; }

    [JsonProperty("support")]
    public SupportInfo? Support { get; set; }
}

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

public class SupportInfo
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}