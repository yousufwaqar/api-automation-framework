using ApiAutomationFramework.DTOs.Response;
using Serilog;

namespace ApiAutomationFramework.Helpers;

public class TokenStore
{
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private readonly ILogger _logger;

    public TokenStore()
    {
        _logger = Log.ForContext<TokenStore>();
    }

    public string GetToken()
    {
        if (_cachedToken == null || DateTime.UtcNow >= _tokenExpiry)
        {
            _logger.Warning("Token not available or expired.");
            return string.Empty;
        }
        return _cachedToken;
    }

    public void SetToken(string token, TimeSpan? validity = null)
    {
        _cachedToken = token;
        _tokenExpiry = DateTime.UtcNow.Add(validity ?? TimeSpan.FromHours(1));
        _logger.Information("Token stored. Expires: {Expiry}", _tokenExpiry);
    }

    public void StoreLoginToken(LoginResponse loginResponse)
    {
        if (loginResponse.Token != null)
        {
            SetToken(loginResponse.Token);
            _logger.Information("Login token stored.");
        }
    }

    public void ClearToken()
    {
        _cachedToken = null;
        _tokenExpiry = DateTime.MinValue;
    }

    public bool HasValidToken() =>
        !string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry;
}
