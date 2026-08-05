using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class PostResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("body")]
    public string Body { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public int UserId { get; set; }
}
