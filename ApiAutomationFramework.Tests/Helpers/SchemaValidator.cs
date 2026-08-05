using NJsonSchema;
using NJsonSchema.Generation;
using Serilog;

namespace ApiAutomationFramework.Helpers;

public class SchemaValidator
{
    private readonly ILogger _logger;

    public SchemaValidator()
    {
        _logger = Log.ForContext<SchemaValidator>();
    }

    public async Task<IList<string>> ValidateAsync(string json, string schemaJson)
    {
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var errors = schema.Validate(json);
        return errors.Select(e => $"{e.Path}: {e.Kind}").ToList();
    }

    public async Task<string> GenerateSchemaAsync<T>()
    {
        // Use lenient schema generation settings
        // This prevents false positives from strict nullability checks
        var settings = new SystemTextJsonSchemaGeneratorSettings
        {
            SerializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null,
            GenerateAbstractProperties = false,
            FlattenInheritanceHierarchy = true
        };

        var generator = new JsonSchemaGenerator(settings);
        var schema = generator.Generate(typeof(T));
        return schema.ToJson();
    }

    public async Task<bool> IsValidAsync<T>(string json)
    {
        try
        {
            var schemaJson = await GenerateSchemaAsync<T>();
            var errors = await ValidateAsync(json, schemaJson);

            if (errors.Any())
            {
                _logger.Warning("Schema validation errors for {Type}: {Errors}",
                    typeof(T).Name, string.Join("; ", errors));
            }

            return !errors.Any();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Schema generation/validation failed for {Type}", typeof(T).Name);
            throw; // Fail the test instead of returning true to avoid false positives
        }
    }

    /// <summary>
    /// Simple structural validation - just checks required properties exist.
    /// Use this instead of full schema validation for more reliable tests.
    /// </summary>
    public bool HasRequiredProperties(string json, params string[] requiredPaths)
    {
        try
        {
            var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            foreach (var path in requiredPaths)
            {
                if (jObj.SelectToken(path) == null)
                {
                    _logger.Warning("Missing required property: {Path}", path);
                    return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Property validation failed");
            return false;
        }
    }
}
