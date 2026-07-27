namespace ApiAutomationFramework.Constants;

public static class HttpHeaders
{
    public const string ContentType = "Content-Type";
    public const string Accept = "Accept";
    public const string Authorization = "Authorization";
    public const string CorrelationId = "X-Correlation-ID";
    public const string RequestSource = "X-Request-Source";

    public static class Values
    {
        public const string ApplicationJson = "application/json";
        public const string BearerPrefix = "Bearer ";
        public const string BasicPrefix = "Basic ";
    }
}