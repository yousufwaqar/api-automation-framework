using Newtonsoft.Json;
using Serilog;

namespace ApiAutomationFramework.Utilities;

public class ReportUtility
{
    private readonly string _reportDirectory;
    private readonly ILogger _logger;
    private readonly List<ScenarioResult> _results = new();
    private readonly object _lock = new();

    public ReportUtility()
    {
        _logger = Log.ForContext<ReportUtility>();
        _reportDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Reports",
            $"Run_{DateTime.UtcNow:yyyyMMdd_HHmmss}");

        Directory.CreateDirectory(_reportDirectory);
    }

    public void CaptureScenarioResult(
        string scenarioTitle,
        string status,
        TimeSpan duration,
        string? errorDetails = null)
    {
        var result = new ScenarioResult
        {
            ScenarioTitle = scenarioTitle,
            Status = status,
            DurationMs = (long)duration.TotalMilliseconds,
            ExecutedAt = DateTime.UtcNow,
            ErrorDetails = errorDetails
        };

        lock (_lock)
        {
            _results.Add(result);
        }
    }

    public void GenerateFinalReport()
    {
        var reportPath = Path.Combine(_reportDirectory, "test-results.json");

        var report = new TestRunReport
        {
            GeneratedAt = DateTime.UtcNow,
            TotalScenarios = _results.Count,
            PassedScenarios = _results.Count(r => r.Status == "PASSED"),
            FailedScenarios = _results.Count(r => r.Status == "FAILED"),
            TotalDurationMs = _results.Sum(r => r.DurationMs),
            Scenarios = _results
        };

        var json = JsonConvert.SerializeObject(report, Formatting.Indented);
        File.WriteAllText(reportPath, json);

        _logger.Information("Report saved: {Path}", reportPath);
        _logger.Information("Results: {Passed}/{Total} passed",
            report.PassedScenarios, report.TotalScenarios);
    }
}

public class ScenarioResult
{
    public string ScenarioTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string? ErrorDetails { get; set; }
}

public class TestRunReport
{
    public DateTime GeneratedAt { get; set; }
    public int TotalScenarios { get; set; }
    public int PassedScenarios { get; set; }
    public int FailedScenarios { get; set; }
    public long TotalDurationMs { get; set; }
    public List<ScenarioResult> Scenarios { get; set; } = new();
}