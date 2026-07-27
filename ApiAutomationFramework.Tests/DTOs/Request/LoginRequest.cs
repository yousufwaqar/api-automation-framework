using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Request;

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;

    public static LoginRequest ValidCredentials() => new()
    {
        Email = "eve.holt@reqres.in",
        Password = "cityslicka"
    };

    public static LoginRequest InvalidCredentials() => new()
    {
        Email = "invalid@test.com",
        Password = "wrongpassword"
    };

    public static LoginRequest MissingPassword() => new()
    {
        Email = "eve.holt@reqres.in",
        Password = string.Empty
    };
}