using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class UserResponse
{
    [JsonProperty("data")]
    public UserData? Data { get; set; }

    [JsonProperty("support")]
    public SupportInfo? Support { get; set; }
}
