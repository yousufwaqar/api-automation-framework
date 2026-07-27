using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

public class UsersListResponse
{
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }

    [JsonProperty("data")]
    public List<UserData> Data { get; set; } = new();

    [JsonProperty("support")]
    public SupportInfo? Support { get; set; }

    [JsonIgnore]
    public bool HasData => Data.Count > 0;
}