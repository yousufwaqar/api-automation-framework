namespace ApiAutomationFramework.Configuration;

public enum TestEnvironment
{
    Development,
    Staging,
    Production
}

public interface IEnvironmentManager
{
    TestEnvironment CurrentEnvironment { get; }
    bool IsProduction();
}

public class EnvironmentManager : IEnvironmentManager
{
    private readonly IConfigurationManager _configManager;

    public EnvironmentManager(IConfigurationManager configManager)
    {
        _configManager = configManager;
    }

    public TestEnvironment CurrentEnvironment
    {
        get
        {
            var envString = _configManager.Settings.Environment;
            return Enum.TryParse<TestEnvironment>(envString, ignoreCase: true, out var env)
                ? env
                : TestEnvironment.Development;
        }
    }

    public bool IsProduction() => CurrentEnvironment == TestEnvironment.Production;
}