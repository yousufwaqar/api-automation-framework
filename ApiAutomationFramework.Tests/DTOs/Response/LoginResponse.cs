using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class LoginResponse
{
    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonIgnore]
    public bool IsSuccessful => !string.IsNullOrEmpty(Token);
}
