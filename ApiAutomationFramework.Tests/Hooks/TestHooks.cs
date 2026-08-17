using ApiAutomationFramework.APIClients.Base;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Constants;
using ApiAutomationFramework.Utilities;
using Reqnroll;
using Reqnroll.UnitTestProvider;
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
    private readonly IEnvironmentManager _environmentManager;
    private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
    private readonly ILogger _logger;

    public TestHooks(
        ScenarioContext scenarioContext,
        IUserApiClient userApiClient,
        IPostApiClient postApiClient,
        ReportUtility reportUtility,
        IEnvironmentManager environmentManager,
        IUnitTestRuntimeProvider unitTestRuntimeProvider)
    {
        _scenarioContext = scenarioContext;
        _userApiClient = userApiClient;
        _postApiClient = postApiClient;
        _reportUtility = reportUtility;
        _environmentManager = environmentManager;
        _unitTestRuntimeProvider = unitTestRuntimeProvider;
        _logger = Log.ForContext<TestHooks>();
    }

    [BeforeScenario(Order = 0)]
    public void SkipScenarioInProduction()
    {
        if (_environmentManager.IsProduction() &&
            _scenarioContext.ScenarioInfo.Tags.Contains(TestTags.SkipInProduction))
        {
            _unitTestRuntimeProvider.TestIgnore(
                $"Scenario '{_scenarioContext.ScenarioInfo.Title}' is skipped in the Production environment.");
        }
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
        // StartTime may be missing if an earlier hook aborted the scenario (e.g. a skip).
        var startTime = _scenarioContext.TryGetValue(ScenarioContextKeys.StartTime, out DateTime start)
            ? start
            : DateTime.UtcNow;
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