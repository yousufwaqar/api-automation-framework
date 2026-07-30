using ApiAutomationFramework.DTOs.Request;

namespace ApiAutomationFramework.Helpers.Builders;

/// <summary>
/// Builder Pattern implementation for creating CreateUserRequest.
/// Enables fluent, readable request construction.
/// Especially useful for complex objects with many optional properties.
/// </summary>
public class UserRequestBuilder
{
    private string _name = "Default User";
    private string _job = "Default Job";

    public UserRequestBuilder WithName(string name)
    {
        _name = name;
        return this;  // Return this to enable chaining
    }

    public UserRequestBuilder WithJob(string job)
    {
        _job = job;
        return this;
    }

    public UserRequestBuilder AsAdmin()
    {
        _job = "Administrator";
        return this;
    }

    public UserRequestBuilder AsGuest()
    {
        _name = "Guest User";
        _job = "Guest";
        return this;
    }

    public UserRequestBuilder WithRandomName()
    {
        var faker = new Bogus.Faker();
        _name = faker.Name.FullName();
        return this;
    }

    public CreateUserRequest Build()
    {
        return new CreateUserRequest
        {
            Name = _name,
            Job = _job
        };
    }
}

/// <summary>
/// Static factory method for cleaner test code.
/// </summary>
public static class UserRequest
{
    public static UserRequestBuilder Create() => new UserRequestBuilder();
}