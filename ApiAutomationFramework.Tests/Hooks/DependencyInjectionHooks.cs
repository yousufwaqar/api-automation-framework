using ApiAutomationFramework.APIClients;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Helpers;
using ApiAutomationFramework.Utilities;
using Reqnroll;
using Reqnroll.BoDi;

namespace ApiAutomationFramework.Hooks;

[Binding]
public class DependencyInjectionHooks
{
    [BeforeTestRun]
    public static void RegisterDependencies(IObjectContainer container)
    {
        // Setup logging first
        LoggingUtility.ConfigureSerilog();

        // Configuration - register as singleton instances
        var configManager = FrameworkConfigurationManager.Instance;
        container.RegisterInstanceAs<IConfigurationManager>(configManager);
        container.RegisterInstanceAs(configManager.Settings);

        // Register RetryHelper as an INSTANCE (not type)
        // This prevents BoDi from trying to resolve int parameters in constructor
        var retryHelper = new RetryHelper(
            configManager.Settings.ApiSettings.ReqRes.RetryCount,
            configManager.Settings.ApiSettings.ReqRes.RetryDelayMilliseconds);
        container.RegisterInstanceAs(retryHelper);

        // Helpers - register as instances to avoid DI resolution issues
        container.RegisterInstanceAs(new JsonHelper());
        container.RegisterInstanceAs(new RandomDataGenerator());
        container.RegisterInstanceAs(new SchemaValidator());
        container.RegisterInstanceAs(new ResponseValidator());
        container.RegisterInstanceAs(new TokenGenerator());

        // Environment Manager
        container.RegisterInstanceAs<IEnvironmentManager>(
            new EnvironmentManager(configManager));

        // API Clients - register as INSTANCES so BoDi doesn't try to construct them
        var userApiClient = new UserApiClient(configManager.Settings, retryHelper);
        container.RegisterInstanceAs<IUserApiClient>(userApiClient);

        var postApiClient = new PostApiClient(configManager.Settings, retryHelper);
        container.RegisterInstanceAs<IPostApiClient>(postApiClient);

        // Report utility
        container.RegisterInstanceAs(new ReportUtility());

        Serilog.Log.Information("DI container configured successfully.");
    }
}