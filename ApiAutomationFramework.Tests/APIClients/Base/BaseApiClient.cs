using System.Diagnostics;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Constants;
using ApiAutomationFramework.Helpers;
using ApiAutomationFramework.Utilities;
using RestSharp;
using Serilog;

namespace ApiAutomationFramework.APIClients.Base;

public abstract class BaseApiClient : IApiClient
{
    protected readonly RestClient _client;
    protected readonly AppSettings _settings;
    protected readonly ILogger _logger;
    protected readonly RetryHelper _retryHelper;
    protected string _correlationId;

    protected BaseApiClient(
     ApiEndpointConfig config,
     AppSettings settings,
     RetryHelper retryHelper,
     string? correlationId = null)
    {
        _settings = settings;
        _retryHelper = retryHelper;
        _correlationId = correlationId ?? Guid.NewGuid().ToString("N")[..8];
        _logger = Log.ForContext(GetType());

        var options = new RestClientOptions(config.BaseUrl)
        {
            Timeout = config.Timeout,   
            FollowRedirects = true,
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false
        };

        _client = new RestClient(options);

        // Add default headers
        foreach (var header in settings.DefaultHeaders)
        {
            _client.AddDefaultHeader(header.Key, header.Value);
        }

        // ═══════════════════════════════════════════════════════════
        // ADD THIS BLOCK - Send API key with every request if configured
        // reqres.in requires x-api-key header since 2024
        // ═══════════════════════════════════════════════════════════
        if (!string.IsNullOrWhiteSpace(config.ApiKey))
        {
            _client.AddDefaultHeader("x-api-key", config.ApiKey);
            _logger.Information("API key configured for: {BaseUrl}", config.BaseUrl);
        }
        // ═══════════════════════════════════════════════════════════

        _logger.Information("Client ready for: {BaseUrl}", config.BaseUrl);
    }

    public RestRequest CreateRequest(string endpoint, Method method)
    {
        var request = new RestRequest(endpoint, method);
        request.AddHeader(HttpHeaders.CorrelationId, _correlationId);
        request.AddHeader(HttpHeaders.RequestSource, "ApiAutomationFramework");
        return request;
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequest(request);

        RestResponse response;
        try
        {
            response = await _retryHelper.ExecuteWithRetryAsync(
                async () => await _client.ExecuteAsync(request));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Request failed: {Method} {Endpoint}",
                request.Method, request.Resource);
            throw new ApiClientException($"Request to {request.Resource} failed", ex);
        }

        stopwatch.Stop();
        LogResponse(response, stopwatch.Elapsed);
        return response;
    }

    public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) where T : class
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequest(request);

        RestResponse<T> response;
        try
        {
            response = await _retryHelper.ExecuteWithRetryAsync(
                async () => await _client.ExecuteAsync<T>(request));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Request failed: {Method} {Endpoint}",
                request.Method, request.Resource);
            throw new ApiClientException($"Request to {request.Resource} failed", ex);
        }

        stopwatch.Stop();
        LogResponse(response, stopwatch.Elapsed);
        return response;
    }

    protected void AddBearerAuth(RestRequest request, string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            request.AddHeader(HttpHeaders.Authorization,
                $"{HttpHeaders.Values.BearerPrefix}{token}");
    }

    public void SetCorrelationId(string correlationId)
    {
        _correlationId = correlationId;
    }

    private void LogRequest(RestRequest request)
    {
        _logger.Information("→ {Method} {BaseUrl}{Endpoint} [CorrId: {CorrelationId}]",
            request.Method, _client.Options.BaseUrl, request.Resource, _correlationId);

        if (_settings.Reporting.IncludeRequestBody)
        {
            var body = request.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if (body != null)
                _logger.Information("  Body: {Body}", body.Value);
        }
    }

    private void LogResponse(RestResponseBase response, TimeSpan elapsed)
    {
        var statusCode = (int)response.StatusCode;
        _logger.Information("← {StatusCode} [{ElapsedMs}ms] [CorrId: {CorrelationId}]",
            statusCode, elapsed.TotalMilliseconds.ToString("F0"), _correlationId);

        if (_settings.Reporting.IncludeResponseBody && !string.IsNullOrEmpty(response.Content))
            _logger.Debug("  Response: {Content}", response.Content);
    }
}