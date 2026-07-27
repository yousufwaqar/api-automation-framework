using ApiAutomationFramework.APIClients.Base;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Utilities;
using Reqnroll;
using Serilog;
using Serilog.Context;

namespace ApiAutomationFramework.Hooks;

[Binding]
public class TestHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IUserApiClient _userApiClient;
    private readonly IPostApiClient _postApiClient;
    private readonly ReportUtility _reportUtility;
    private readonly ILogger _logger;

    public TestHooks(
        ScenarioContext scenarioContext,
        IUserApiClient userApiClient,
        IPostApiClient postApiClient,
        ReportUtility reportUtility)
    {
        _scenarioContext = scenarioContext;
        _userApiClient = userApiClient;
        _postApiClient = postApiClient;
        _reportUtility = reportUtility;
        _logger = Log.ForContext<TestHooks>();
    }

    [BeforeScenario(Order = 1)]
    public void BeforeScenario()
    {
        var correlationId = $"test-{Guid.NewGuid():N}"[..16];
        _scenarioContext[ScenarioContextKeys.CorrelationId] = correlationId;
        _scenarioContext[ScenarioContextKeys.StartTime] = DateTime.UtcNow;

        LogContext.PushProperty("CorrelationId", correlationId);
        LogContext.PushProperty("ScenarioTitle", _scenarioContext.ScenarioInfo.Title);

        if (_userApiClient is BaseApiClient userBase)
            userBase.SetCorrelationId(correlationId);

        if (_postApiClient is BaseApiClient postBase)
            postBase.SetCorrelationId(correlationId);

        _logger.Information("═══ SCENARIO START: {Title} ═══",
            _scenarioContext.ScenarioInfo.Title);
        _logger.Information("Tags: {Tags}",
            string.Join(", ", _scenarioContext.ScenarioInfo.Tags));
    }

    [AfterScenario(Order = 1)]
    public void AfterScenario()
    {
        var startTime = (DateTime)_scenarioContext[ScenarioContextKeys.StartTime];
        var duration = DateTime.UtcNow - startTime;
        var status = _scenarioContext.TestError == null ? "PASSED" : "FAILED";

        _logger.Information("═══ SCENARIO END: {Status} in {Ms}ms ═══",
            status, duration.TotalMilliseconds.ToString("F0"));

        if (_scenarioContext.TestError != null)
        {
            _logger.Error(_scenarioContext.TestError,
                "FAILED: {Message}", _scenarioContext.TestError.Message);
        }

        _reportUtility.CaptureScenarioResult(
            _scenarioContext.ScenarioInfo.Title,
            status,
            duration,
            _scenarioContext.TestError?.ToString());
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        LoggingUtility.FlushAndClose();
    }
}