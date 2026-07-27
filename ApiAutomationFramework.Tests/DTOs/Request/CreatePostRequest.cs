using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Request;

public class CreatePostRequest
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("body")]
    public string Body { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public int UserId { get; set; }

    public static CreatePostRequest Default() => new()
    {
        Title = "Test Post Title",
        Body = "Test post body content",
        UserId = 1
    };
}