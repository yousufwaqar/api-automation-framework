using ApiAutomationFramework.DTOs.Request;

namespace ApiAutomationFramework.Helpers.Factories;

/// <summary>
/// Factory interface for creating test data objects.
/// Implements the Factory Pattern to centralize object creation.
/// </summary>
public interface ITestDataFactory
{
    CreateUserRequest CreateValidUser();
    CreateUserRequest CreateUserWithLongName();
    CreateUserRequest CreateUserWithSpecialCharacters();
    CreateUserRequest CreateInvalidUser();
    List<CreateUserRequest> CreateBulkUsers(int count);

    CreatePostRequest CreateValidPost(int userId = 1);
    CreatePostRequest CreatePostWithHtmlContent(int userId = 1);
    List<CreatePostRequest> CreateBulkPosts(int userId, int count);

    LoginRequest CreateValidLoginRequest();
    LoginRequest CreateExpiredCredentialsLogin();
}