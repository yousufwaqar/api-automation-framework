namespace ApiAutomationFramework.Configuration;

public class AppSettings
{
    public string Environment { get; set; } = "Development";
    public ApiSettingsConfig ApiSettings { get; set; } = new();
    public AuthenticationConfig Authentication { get; set; } = new();
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public ReportingConfig Reporting { get; set; } = new();
}

public class ApiSettingsConfig
{
    public ApiEndpointConfig ReqRes { get; set; } = new();
    public ApiEndpointConfig JsonPlaceholder { get; set; } = new();
}

public class ApiEndpointConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
    public string ApiKey { get; set; } = string.Empty;  
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);
}

public class AuthenticationConfig
{
    public string BearerToken { get; set; } = string.Empty;
    public BasicAuthConfig BasicAuth { get; set; } = new();
    public string ApiKey { get; set; } = string.Empty;
}

public class BasicAuthConfig
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ReportingConfig
{
    public string OutputDirectory { get; set; } = "Reports";
    public bool IncludeRequestBody { get; set; } = true;
    public bool IncludeResponseBody { get; set; } = true;
    public bool IncludeHeaders { get; set; } = true;
}