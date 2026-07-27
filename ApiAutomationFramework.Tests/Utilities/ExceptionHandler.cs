namespace ApiAutomationFramework.Utilities;

public class FrameworkException : Exception
{
    public FrameworkException(string message) : base(message) { }
    public FrameworkException(string message, Exception inner) : base(message, inner) { }
}

public class ApiClientException : FrameworkException
{
    public ApiClientException(string message) : base(message) { }
    public ApiClientException(string message, Exception inner) : base(message, inner) { }
}

public class ApiAuthenticationException : ApiClientException
{
    public ApiAuthenticationException(string message) : base(message) { }
}