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
