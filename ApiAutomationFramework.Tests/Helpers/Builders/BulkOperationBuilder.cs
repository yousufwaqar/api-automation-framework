using ApiAutomationFramework.DTOs.Request;
using Bogus;

namespace ApiAutomationFramework.Helpers.Builders;

/// <summary>
/// Builder for complex nested payload with arrays.
/// Shows how to construct real-world enterprise API requests.
/// </summary>
public class BulkOperationBuilder
{
    private readonly BulkOperationRequest _request = new();
    private readonly Faker _faker = new();

    public BulkOperationBuilder WithOperation(string operation)
    {
        _request.Operation = operation;
        return this;
    }

    public BulkOperationBuilder WithSource(string source)
    {
        _request.Metadata.Source = source;
        return this;
    }

    public BulkOperationBuilder AddUser(Action<ComplexUserData> configure)
    {
        var user = new ComplexUserData
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Email = _faker.Internet.Email()
        };
        configure(user);
        _request.Users.Add(user);
        return this;
    }

    public BulkOperationBuilder AddRandomUsers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddUser(user =>
            {
                user.Addresses.Add(new Address
                {
                    Type = "home",
                    Street = _faker.Address.StreetAddress(),
                    City = _faker.Address.City(),
                    Country = _faker.Address.Country(),
                    PostalCode = _faker.Address.ZipCode(),
                    IsPrimary = true
                });
                user.Roles.Add("user");
            });
        }
        return this;
    }

    public BulkOperationBuilder AddAdminPermission()
    {
        _request.Permissions.Add(new Permission
        {
            Resource = "users",
            Actions = new List<string> { "read", "write", "delete", "admin" },
            Conditions = new Dictionary<string, string>
            {
                { "environment", "production" },
                { "region", "any" }
            }
        });
        return this;
    }

    public BulkOperationBuilder AddReadOnlyPermission()
    {
        _request.Permissions.Add(new Permission
        {
            Resource = "users",
            Actions = new List<string> { "read" }
        });
        return this;
    }

    public BulkOperationBuilder WithTags(params string[] tags)
    {
        _request.Tags.AddRange(tags);
        return this;
    }

    public BulkOperationRequest Build() => _request;
}