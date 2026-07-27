using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;

namespace ApiAutomationFramework.Helpers;

public class RetryHelper
{
    private readonly int _retryCount;
    private readonly int _delayMilliseconds;
    private readonly ILogger _logger;

    public RetryHelper(int retryCount = 3, int delayMilliseconds = 1000)
    {
        _retryCount = retryCount;
        _delayMilliseconds = delayMilliseconds;
        _logger = Log.ForContext<RetryHelper>();
    }

    public async Task<RestResponse> ExecuteWithRetryAsync(Func<Task<RestResponse>> action)
    {
        var policy = Policy
            .HandleResult<RestResponse>(ShouldRetry)
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: _retryCount,
                sleepDurationProvider: attempt =>
                {
                    var baseDelay = TimeSpan.FromMilliseconds(_delayMilliseconds * Math.Pow(2, attempt - 1));
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 200));
                    return baseDelay + jitter;
                },
                onRetry: (outcome, delay, attempt, context) =>
                {
                    _logger.Warning("Retry {Attempt}/{Max} after {Delay}ms",
                        attempt, _retryCount, delay.TotalMilliseconds);
                });

        return await policy.ExecuteAsync(action);
    }

    public async Task<RestResponse<T>> ExecuteWithRetryAsync<T>(Func<Task<RestResponse<T>>> action)
    {
        var policy = Policy
            .HandleResult<RestResponse<T>>(r => ShouldRetry(r))
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: _retryCount,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(_delayMilliseconds * Math.Pow(2, attempt - 1)),
                onRetry: (outcome, delay, attempt, context) =>
                {
                    _logger.Warning("Retry {Attempt}/{Max} after {Delay}ms",
                        attempt, _retryCount, delay.TotalMilliseconds);
                });

        return await policy.ExecuteAsync(action);
    }

    private static bool ShouldRetry(RestResponseBase response)
    {
        if (response == null) return true;
        var status = (int)response.StatusCode;
        return status == 0 || status == 408 || status == 429 ||
               status == 500 || status == 502 || status == 503 || status == 504;
    }
}