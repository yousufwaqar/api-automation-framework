using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class SupportInfo
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}
