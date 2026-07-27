using FluentAssertions;
using RestSharp;
using Serilog;
using System.Net;

namespace ApiAutomationFramework.Helpers;

public class ResponseValidator
{
    private readonly ILogger _logger;

    public ResponseValidator()
    {
        _logger = Log.ForContext<ResponseValidator>();
    }

    public ResponseValidator ValidateSuccess(RestResponseBase response)
    {
        var statusCode = (int)response.StatusCode;
        statusCode.Should().BeInRange(200, 299,
            because: $"Expected success but got {statusCode}: {response.Content}");
        return this;
    }

    public ResponseValidator ValidateStatusCode(RestResponseBase response, HttpStatusCode expected)
    {
        response.StatusCode.Should().Be(expected,
            because: $"Response: {response.Content}");
        return this;
    }

    public ResponseValidator ValidateNotEmpty(RestResponseBase response)
    {
        response.Content.Should().NotBeNullOrWhiteSpace(
            because: "API should return a response body");
        return this;
    }

    public ResponseValidator ValidateJsonContentType(RestResponseBase response)
    {
        response.ContentType.Should().Contain("application/json",
            because: "API should return JSON content type");
        return this;
    }

    public ResponseValidator ValidateHeader(
        RestResponseBase response,
        string headerName,
        string? expectedValue = null)
    {
        var header = response.Headers?.FirstOrDefault(h =>
            h.Name?.Equals(headerName, StringComparison.OrdinalIgnoreCase) == true);

        header.Should().NotBeNull(because: $"Response should have header '{headerName}'");

        if (expectedValue != null)
        {
            header!.Value?.ToString().Should().Be(expectedValue);
        }

        return this;
    }
}