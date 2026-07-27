using ApiAutomationFramework.DTOs.Request;
using Bogus;

namespace ApiAutomationFramework.Helpers;

public class RandomDataGenerator
{
    private readonly Faker _faker;

    public RandomDataGenerator()
    {
        _faker = new Faker("en");
    }

    public CreateUserRequest GenerateCreateUserRequest()
    {
        return new CreateUserRequest
        {
            Name = _faker.Name.FullName(),
            Job = _faker.Name.JobTitle()
        };
    }

    public UpdateUserRequest GenerateUpdateUserRequest()
    {
        return new UpdateUserRequest
        {
            Name = _faker.Name.FullName(),
            Job = _faker.Name.JobTitle()
        };
    }

    public CreatePostRequest GenerateCreatePostRequest(int userId = 1)
    {
        return new CreatePostRequest
        {
            Title = _faker.Lorem.Sentence(),
            Body = _faker.Lorem.Paragraph(),
            UserId = userId
        };
    }

    public string GenerateEmail() => _faker.Internet.Email();
    public string GenerateName() => _faker.Name.FullName();
    public string GenerateJobTitle() => _faker.Name.JobTitle();
    public int GeneratePositiveInt(int min = 1, int max = 100) => _faker.Random.Int(min, max);
    public string GenerateGuid() => Guid.NewGuid().ToString();
}