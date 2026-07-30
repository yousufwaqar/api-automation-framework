using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Response;

/// <summary>
/// Complex response with nested arrays and success/failure tracking.
/// </summary>
public class BulkOperationResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("totalProcessed")]
    public int TotalProcessed { get; set; }

    [JsonProperty("successCount")]
    public int SuccessCount { get; set; }

    [JsonProperty("failureCount")]
    public int FailureCount { get; set; }

    [JsonProperty("results")]
    public List<OperationResult> Results { get; set; } = new();

    [JsonProperty("errors")]
    public List<OperationError> Errors { get; set; } = new();

    [JsonProperty("summary")]
    public OperationSummary Summary { get; set; } = new();

    // Computed properties for easy access
    [JsonIgnore]
    public double SuccessRate =>
        TotalProcessed == 0 ? 0 : (double)SuccessCount / TotalProcessed * 100;

    [JsonIgnore]
    public List<string> FailedEmails =>
        Errors.Select(e => e.Email).ToList();

    [JsonIgnore]
    public List<OperationResult> SuccessfulResults =>
        Results.Where(r => r.Status == "success").ToList();
}

public class OperationResult
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("permissions")]
    public List<string> Permissions { get; set; } = new();
}

public class OperationError
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("field")]
    public string? Field { get; set; }
}

public class OperationSummary
{
    [JsonProperty("startTime")]
    public DateTime StartTime { get; set; }

    [JsonProperty("endTime")]
    public DateTime EndTime { get; set; }

    [JsonProperty("durationMs")]
    public long DurationMs { get; set; }

    [JsonProperty("processingRate")]
    public double ProcessingRate { get; set; }
}