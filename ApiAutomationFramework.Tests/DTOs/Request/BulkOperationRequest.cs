using Newtonsoft.Json;

namespace ApiAutomationFramework.DTOs.Request;

/// <summary>
/// Complex nested payload with arrays and objects.
/// Demonstrates handling of real-world enterprise API structures.
/// </summary>
public class BulkOperationRequest
{
    [JsonProperty("operation")]
    public string Operation { get; set; } = "create";

    [JsonProperty("metadata")]
    public OperationMetadata Metadata { get; set; } = new();

    [JsonProperty("users")]
    public List<ComplexUserData> Users { get; set; } = new();

    [JsonProperty("permissions")]
    public List<Permission> Permissions { get; set; } = new();

    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = new();
}

public class OperationMetadata
{
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonProperty("source")]
    public string Source { get; set; } = "ApiAutomationFramework";

    [JsonProperty("environment")]
    public string Environment { get; set; } = "test";
}

public class ComplexUserData
{
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("addresses")]
    public List<Address> Addresses { get; set; } = new();

    [JsonProperty("roles")]
    public List<string> Roles { get; set; } = new();

    [JsonProperty("preferences")]
    public UserPreferences Preferences { get; set; } = new();

    [JsonProperty("customFields")]
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

public class Address
{
    [JsonProperty("type")]
    public string Type { get; set; } = "home";  // home, work, billing

    [JsonProperty("street")]
    public string Street { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonProperty("isPrimary")]
    public bool IsPrimary { get; set; }
}

public class UserPreferences
{
    [JsonProperty("language")]
    public string Language { get; set; } = "en";

    [JsonProperty("timezone")]
    public string Timezone { get; set; } = "UTC";

    [JsonProperty("notifications")]
    public NotificationSettings Notifications { get; set; } = new();
}

public class NotificationSettings
{
    [JsonProperty("email")]
    public bool Email { get; set; } = true;

    [JsonProperty("sms")]
    public bool Sms { get; set; }

    [JsonProperty("push")]
    public bool Push { get; set; } = true;
}

public class Permission
{
    [JsonProperty("resource")]
    public string Resource { get; set; } = string.Empty;

    [JsonProperty("actions")]
    public List<string> Actions { get; set; } = new();

    [JsonProperty("conditions")]
    public Dictionary<string, string> Conditions { get; set; } = new();
}