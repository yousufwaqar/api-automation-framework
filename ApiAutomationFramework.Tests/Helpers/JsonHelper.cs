using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ApiAutomationFramework.Helpers;

public class JsonHelper
{
    private readonly ILogger _logger;

    private static readonly JsonSerializerSettings _settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Formatting = Formatting.Indented
    };

    public JsonHelper()
    {
        _logger = Log.ForContext<JsonHelper>();
    }

    public T? Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            _logger.Warning("Attempted to deserialize empty JSON to {Type}", typeof(T).Name);
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "Failed to deserialize JSON to {Type}", typeof(T).Name);
            throw;
        }
    }

    public string Serialize<T>(T obj) where T : class
    {
        return JsonConvert.SerializeObject(obj, _settings);
    }

    public string? GetProperty(string json, string propertyPath)
    {
        try
        {
            var token = JObject.Parse(json).SelectToken(propertyPath);
            return token?.ToString();
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Could not get property {Path}", propertyPath);
            return null;
        }
    }

    public bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T? ReadTestDataFile<T>(string fileName) where T : class
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Test data file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        return Deserialize<T>(json);
    }
}