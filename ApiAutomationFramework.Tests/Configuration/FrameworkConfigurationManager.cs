using Microsoft.Extensions.Configuration;
using Serilog;

namespace ApiAutomationFramework.Configuration;

public interface IConfigurationManager
{
    AppSettings Settings { get; }
    string GetEnvironment();
    ApiEndpointConfig GetApiConfig(string apiName);
}

public class FrameworkConfigurationManager : IConfigurationManager
{
    private readonly AppSettings _settings;

    public FrameworkConfigurationManager()
    {
        var environment = System.Environment.GetEnvironmentVariable("TEST_ENVIRONMENT")
                         ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        _settings = new AppSettings();
        configuration.Bind(_settings);

        // TEST_ENVIRONMENT selects the config file AND the active environment,
        // so the framework can react to it (e.g. skip scenarios tagged @skipInProduction).
        var testEnvironment = System.Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(testEnvironment))
        {
            _settings.Environment = testEnvironment;
        }
    }

    public AppSettings Settings => _settings;

    public string GetEnvironment() => _settings.Environment;

    public ApiEndpointConfig GetApiConfig(string apiName)
    {
        return apiName switch
        {
            "ReqRes" => _settings.ApiSettings.ReqRes,
            "JsonPlaceholder" => _settings.ApiSettings.JsonPlaceholder,
            _ => throw new ArgumentException($"Unknown API: {apiName}")
        };
    }
}
