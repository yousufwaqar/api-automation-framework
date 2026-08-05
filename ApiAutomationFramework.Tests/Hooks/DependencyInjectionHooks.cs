using ApiAutomationFramework.APIClients;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Helpers;
using ApiAutomationFramework.Helpers.Facades;
using ApiAutomationFramework.Helpers.Factories;
using ApiAutomationFramework.Helpers.Selectors;
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
        // Configuration
        var configManager = new FrameworkConfigurationManager();
        container.RegisterInstanceAs<IConfigurationManager>(configManager);

        var settings = configManager.Settings;
        container.RegisterInstanceAs(settings);

        // Setup logging first
        LoggingUtility.ConfigureSerilog(settings);

        // RetryHelper
        var retryHelper = new RetryHelper(
            settings.ApiSettings.ReqRes.RetryCount,
            settings.ApiSettings.ReqRes.RetryDelayMilliseconds);
        container.RegisterInstanceAs(retryHelper);

        // Helpers
        container.RegisterInstanceAs(new JsonHelper());
        container.RegisterInstanceAs(new RandomDataGenerator());
        container.RegisterInstanceAs(new SchemaValidator());
        container.RegisterInstanceAs(new ResponseValidator());
        container.RegisterInstanceAs(new TokenStore());

        // ═══════════════════════════════════════════════════════
        // Factory Pattern - Create BEFORE it's used
        // ═══════════════════════════════════════════════════════
        var testDataFactory = new TestDataFactory();
        container.RegisterInstanceAs<ITestDataFactory>(testDataFactory);

        // Environment Manager
        container.RegisterInstanceAs<IEnvironmentManager>(
            new EnvironmentManager(configManager));

        // API Clients
        var userApiClient = new UserApiClient(settings, retryHelper);
        container.RegisterInstanceAs<IUserApiClient>(userApiClient);

        var postApiClient = new PostApiClient(settings, retryHelper);
        container.RegisterInstanceAs<IPostApiClient>(postApiClient);

        // ═══════════════════════════════════════════════════════
        // Selectors
        // ═══════════════════════════════════════════════════════
        container.RegisterInstanceAs(new ResponseSelector());

        // ═══════════════════════════════════════════════════════
        // Facade Pattern - Now testDataFactory exists
        // ═══════════════════════════════════════════════════════
        var facade = new ApiTestFacade(userApiClient, postApiClient, testDataFactory);
        container.RegisterInstanceAs(facade);

        // Report utility
        container.RegisterInstanceAs(new ReportUtility());
        Serilog.Log.Information("DI container configured successfully.");
    }
}
