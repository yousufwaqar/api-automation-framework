using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class ErrorResponse
{
    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("status")]
    public int? Status { get; set; }
}

public class LoginResponse
{
    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonIgnore]
    public bool IsSuccessful => !string.IsNullOrEmpty(Token);
}