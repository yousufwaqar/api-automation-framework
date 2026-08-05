using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using ApiAutomationFramework.Helpers.Factories;
using RestSharp;
using Serilog;

namespace ApiAutomationFramework.Helpers.Facades;

/// <summary>
/// Facade Pattern implementation.
/// Provides a simplified interface to complex API operations.
/// Hides the complexity of multiple API calls behind single method calls.
/// </summary>
public class ApiTestFacade
{
    private readonly IUserApiClient _userApiClient;
    private readonly IPostApiClient _postApiClient;
    private readonly ITestDataFactory _dataFactory;
    private readonly ILogger _logger;

    public ApiTestFacade(
        IUserApiClient userApiClient,
        IPostApiClient postApiClient,
        ITestDataFactory dataFactory)
    {
        _userApiClient = userApiClient;
        _postApiClient = postApiClient;
        _dataFactory = dataFactory;
        _logger = Log.ForContext<ApiTestFacade>();
    }

    /// <summary>
    /// FACADE METHOD: Complete user lifecycle in one call.
    /// Hides: Create → Verify → Update → Verify → Delete
    /// Client only calls one method instead of five separate API calls.
    /// </summary>
    public async Task<UserLifecycleResult> ExecuteFullUserLifecycleAsync()
    {
        _logger.Information("Facade: Starting full user lifecycle test");
        var result = new UserLifecycleResult();

        // Step 1: Create user
        var createRequest = _dataFactory.CreateValidUser();
        var createResponse = await _userApiClient.CreateUserAsync(createRequest);
        result.CreateStatusCode = (int)createResponse.StatusCode;
        result.CreatedUserId = createResponse.Data?.Id;
        
        int userId = 2; // Default fallback
        if (createResponse.Data?.Id != null && int.TryParse(createResponse.Data.Id, out var parsedId))
        {
            userId = parsedId;
        }

        // Step 2: Update user
        var updateRequest = new UpdateUserRequest
        {
            Name = createRequest.Name + " Updated",
            Job = "Senior " + createRequest.Job
        };
        var updateResponse = await _userApiClient.UpdateUserAsync(userId, updateRequest);
        result.UpdateStatusCode = (int)updateResponse.StatusCode;

        // Step 3: Delete user
        var deleteResponse = await _userApiClient.DeleteUserAsync(userId);
        result.DeleteStatusCode = (int)deleteResponse.StatusCode;

        result.AllOperationsSuccessful =
            result.CreateStatusCode == 201 &&
            result.UpdateStatusCode == 200 &&
            result.DeleteStatusCode == 204;

        _logger.Information("Facade: Lifecycle completed. Success: {Success}",
            result.AllOperationsSuccessful);

        return result;
    }

    /// <summary>
    /// FACADE METHOD: Login and get authenticated response in one call.
    /// Hides: Login → Extract Token → Make Authenticated Request
    /// </summary>
    public async Task<AuthenticatedRequestResult> LoginAndGetUsersAsync(
        string email, string password)
    {
        _logger.Information("Facade: Login and fetch users for {Email}", email);

        // Step 1: Login
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var loginResponse = await _userApiClient.LoginAsync(loginRequest);

        var result = new AuthenticatedRequestResult
        {
            LoginStatusCode = (int)loginResponse.StatusCode,
            Token = loginResponse.Data?.Token
        };

        // Step 2: If login successful, fetch users
        if (loginResponse.IsSuccessful && !string.IsNullOrEmpty(result.Token))
        {
            var usersResponse = await _userApiClient.GetUsersAsync();
            result.UsersStatusCode = (int)usersResponse.StatusCode;
            result.UserCount = usersResponse.Data?.Data.Count ?? 0;
        }

        return result;
    }

    /// <summary>
    /// FACADE METHOD: Create user and multiple posts in one operation.
    /// Hides: Create User → Create Multiple Posts → Link Posts to User
    /// </summary>
    public async Task<BulkCreationResult> CreateUserWithPostsAsync(int postCount)
    {
        _logger.Information("Facade: Creating user with {Count} posts", postCount);

        var result = new BulkCreationResult();

        // Create user
        var userRequest = _dataFactory.CreateValidUser();
        var userResponse = await _userApiClient.CreateUserAsync(userRequest);
        result.UserCreated = userResponse.IsSuccessful;
        result.UserId = int.TryParse(userResponse.Data?.Id, out var id) ? id : 1;

        // Create multiple posts for the user
        var posts = _dataFactory.CreateBulkPosts(result.UserId, postCount);
        foreach (var post in posts)
        {
            var postResponse = await _postApiClient.CreatePostAsync(post);
            if (postResponse.IsSuccessful)
                result.PostsCreated++;
        }

        result.AllSuccessful = result.UserCreated && result.PostsCreated == postCount;
        return result;
    }
}

// ═══════════════════════════════════════════════════
// RESULT DTOs FOR FACADE OPERATIONS
// ═══════════════════════════════════════════════════

public class UserLifecycleResult
{
    public int CreateStatusCode { get; set; }
    public string? CreatedUserId { get; set; }
    public int UpdateStatusCode { get; set; }
    public int DeleteStatusCode { get; set; }
    public bool AllOperationsSuccessful { get; set; }
}

public class AuthenticatedRequestResult
{
    public int LoginStatusCode { get; set; }
    public string? Token { get; set; }
    public int UsersStatusCode { get; set; }
    public int UserCount { get; set; }
}

public class BulkCreationResult
{
    public bool UserCreated { get; set; }
    public int UserId { get; set; }
    public int PostsCreated { get; set; }
    public bool AllSuccessful { get; set; }
}
