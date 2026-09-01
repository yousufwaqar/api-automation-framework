using ApiAutomationFramework.DTOs.Request;
using Bogus;

namespace ApiAutomationFramework.Helpers.Factories;

/// <summary>
/// Concrete implementation of the Factory Pattern.
/// Encapsulates creation logic for all test data objects.
/// This centralizes test data creation and makes tests more maintainable.
/// </summary>
public class TestDataFactory : ITestDataFactory
{
    private readonly Faker _faker;

    public TestDataFactory()
    {
        _faker = new Faker("en");
    }

    // ═══════════════════════════════════════════════════
    // USER FACTORY METHODS
    // ═══════════════════════════════════════════════════

    public CreateUserRequest CreateValidUser()
    {
        return new CreateUserRequest
        {
            Name = _faker.Name.FullName(),
            Job = _faker.Name.JobTitle()
        };
    }

    public CreateUserRequest CreateUserWithLongName()
    {
        return new CreateUserRequest
        {
            Name = new string('A', 500),  // 500 character name
            Job = _faker.Name.JobTitle()
        };
    }

    public CreateUserRequest CreateUserWithSpecialCharacters()
    {
        return new CreateUserRequest
        {
            Name = "O'Brien-Smith \u00e9\u00f1\u00fc",
            Job = "Sr. Engineer & Team Lead"
        };
    }

    public CreateUserRequest CreateXssPayloadUser()
    {
        return new CreateUserRequest
        {
            Name = "<script>alert('xss')</script>",
            Job = "<img src=x onerror=alert(1)>"
        };
    }

    public CreateUserRequest CreateInvalidUser()
    {
        return new CreateUserRequest
        {
            Name = string.Empty,  // Empty name - should fail validation
            Job = string.Empty
        };
    }

    public List<CreateUserRequest> CreateBulkUsers(int count)
    {
        var users = new List<CreateUserRequest>();
        for (int i = 0; i < count; i++)
        {
            users.Add(CreateValidUser());
        }
        return users;
    }

    // ═══════════════════════════════════════════════════
    // POST FACTORY METHODS
    // ═══════════════════════════════════════════════════

    public CreatePostRequest CreateValidPost(int userId = 1)
    {
        return new CreatePostRequest
        {
            Title = _faker.Lorem.Sentence(),
            Body = _faker.Lorem.Paragraphs(3),
            UserId = userId
        };
    }

    public CreatePostRequest CreatePostWithHtmlContent(int userId = 1)
    {
        return new CreatePostRequest
        {
            Title = "<h1>HTML Title</h1>",
            Body = "<script>alert('test')</script><p>Body with <strong>HTML</strong></p>",
            UserId = userId
        };
    }

    public List<CreatePostRequest> CreateBulkPosts(int userId, int count)
    {
        var posts = new List<CreatePostRequest>();
        for (int i = 0; i < count; i++)
        {
            posts.Add(CreateValidPost(userId));
        }
        return posts;
    }

    // ═══════════════════════════════════════════════════
    // AUTHENTICATION FACTORY METHODS
    // ═══════════════════════════════════════════════════

    public LoginRequest CreateValidLoginRequest()
    {
        return LoginRequest.ValidCredentials();
    }

    public LoginRequest CreateExpiredCredentialsLogin()
    {
        return new LoginRequest
        {
            Email = "expired@test.com",
            Password = "expired123"
        };
    }
}