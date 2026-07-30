using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using ApiAutomationFramework.Helpers;
using ApiAutomationFramework.Helpers.Builders;
using ApiAutomationFramework.Helpers.Facades;
using ApiAutomationFramework.Helpers.Factories;
using ApiAutomationFramework.Helpers.Selectors;
using ApiAutomationFramework.Hooks;
using FluentAssertions;
using Reqnroll;
using RestSharp;
using Serilog;

namespace ApiAutomationFramework.StepDefinitions;

[Binding]
public class AdvancedPatternsStepDefinitions
{
    private readonly IUserApiClient _userApiClient;
    private readonly IPostApiClient _postApiClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly JsonHelper _jsonHelper;
    private readonly ITestDataFactory _testDataFactory;
    private readonly ApiTestFacade _apiTestFacade;
    private readonly ResponseSelector _responseSelector;
    private readonly ILogger _logger;

    private CreateUserRequest? _createUserRequest;
    private UserLifecycleResult? _lifecycleResult;
    private AuthenticatedRequestResult? _authResult;
    private List<UserData>? _filteredUsers;
    private List<PostResponse>? _filteredPosts;

    public AdvancedPatternsStepDefinitions(
        IUserApiClient userApiClient,
        IPostApiClient postApiClient,
        ScenarioContext scenarioContext,
        JsonHelper jsonHelper,
        ITestDataFactory testDataFactory,
        ApiTestFacade apiTestFacade,
        ResponseSelector responseSelector)
    {
        _userApiClient = userApiClient;
        _postApiClient = postApiClient;
        _scenarioContext = scenarioContext;
        _jsonHelper = jsonHelper;
        _testDataFactory = testDataFactory;
        _apiTestFacade = apiTestFacade;
        _responseSelector = responseSelector;
        _logger = Log.ForContext<AdvancedPatternsStepDefinitions>();
    }

    // ═══════════════════════════════════════════════════
    // FACTORY PATTERN STEPS
    // ═══════════════════════════════════════════════════

    [Given("I create a valid user using the test data factory")]
    public void GivenICreateValidUserUsingFactory()
    {
        _createUserRequest = _testDataFactory.CreateValidUser();
        _scenarioContext["CreateUserRequest"] = _createUserRequest;
        _logger.Information("Factory created user: Name={Name}, Job={Job}",
            _createUserRequest.Name, _createUserRequest.Job);
    }

    [Given("I create a user with special characters using the factory")]
    public void GivenICreateUserWithSpecialCharactersUsingFactory()
    {
        _createUserRequest = _testDataFactory.CreateUserWithSpecialCharacters();
        _scenarioContext["CreateUserRequest"] = _createUserRequest;
        _logger.Information("Factory created user with special chars: Name={Name}",
            _createUserRequest.Name);
    }

    // ═══════════════════════════════════════════════════
    // BUILDER PATTERN STEPS
    // ═══════════════════════════════════════════════════

    [Given("I build an admin user request with name {string}")]
    public void GivenIBuildAdminUserRequest(string name)
    {
        _createUserRequest = UserRequest.Create()
            .WithName(name)
            .AsAdmin()
            .Build();

        _scenarioContext["CreateUserRequest"] = _createUserRequest;
        _logger.Information("Builder created admin user: Name={Name}",
            _createUserRequest.Name);
    }

    [Given("I build a user request with random name and job {string}")]
    public void GivenIBuildUserWithRandomNameAndJob(string job)
    {
        _createUserRequest = UserRequest.Create()
            .WithRandomName()
            .WithJob(job)
            .Build();

        _scenarioContext["CreateUserRequest"] = _createUserRequest;
        _logger.Information("Builder created user: Name={Name}, Job={Job}",
            _createUserRequest.Name, _createUserRequest.Job);
    }

    // ═══════════════════════════════════════════════════
    // SHARED WHEN STEP - Sends POST using the prepared request from ScenarioContext
    // ═══════════════════════════════════════════════════

    [When("I send a POST request to create the user using the prepared request")]
    public async Task WhenISendPostRequestUsingPreparedRequest()
    {
        var request = _scenarioContext["CreateUserRequest"] as CreateUserRequest;
        request.Should().NotBeNull(because: "A Given step must prepare the request first");

        var response = await _userApiClient.CreateUserAsync(request!);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    // ═══════════════════════════════════════════════════
    // FACADE PATTERN STEPS
    // ═══════════════════════════════════════════════════

    [When("I execute the full user lifecycle through the facade")]
    public async Task WhenIExecuteFullUserLifecycleThroughFacade()
    {
        _lifecycleResult = await _apiTestFacade.ExecuteFullUserLifecycleAsync();
        _logger.Information("Facade lifecycle result: Success={Success}",
            _lifecycleResult.AllOperationsSuccessful);
    }

    [When("I login and fetch users through the facade with valid credentials")]
    public async Task WhenILoginAndFetchUsersThroughFacade()
    {
        _authResult = await _apiTestFacade.LoginAndGetUsersAsync(
            "eve.holt@reqres.in", "cityslicka");

        _logger.Information("Facade auth result: LoginStatus={LoginStatus}, UserCount={UserCount}",
            _authResult.LoginStatusCode, _authResult.UserCount);
    }

    [Then("all lifecycle operations should be successful")]
    public void ThenAllLifecycleOperationsShouldBeSuccessful()
    {
        _lifecycleResult.Should().NotBeNull();
        _lifecycleResult!.AllOperationsSuccessful.Should().BeTrue(
            because: "Facade should successfully execute all lifecycle operations");
    }

    [Then("the create operation should return status {int}")]
    public void ThenCreateOperationShouldReturnStatus(int expectedStatus)
    {
        _lifecycleResult.Should().NotBeNull();
        _lifecycleResult!.CreateStatusCode.Should().Be(expectedStatus);
    }

    [Then("the update operation should return status {int}")]
    public void ThenUpdateOperationShouldReturnStatus(int expectedStatus)
    {
        _lifecycleResult.Should().NotBeNull();
        _lifecycleResult!.UpdateStatusCode.Should().Be(expectedStatus);
    }

    [Then("the login should be successful")]
    public void ThenLoginShouldBeSuccessful()
    {
        _authResult.Should().NotBeNull();
        _authResult!.LoginStatusCode.Should().Be(200);
        _authResult.Token.Should().NotBeNullOrEmpty();
    }

    [Then("the users list should not be empty")]
    public void ThenUsersListShouldNotBeEmpty()
    {
        _authResult.Should().NotBeNull();
        _authResult!.UserCount.Should().BeGreaterThan(0,
            because: "Facade should fetch users after successful login");
    }

    // ═══════════════════════════════════════════════════
    // SELECTOR PATTERN STEPS
    // ═══════════════════════════════════════════════════

    [Then("I should be able to filter users by {string} email domain")]
    public void ThenIShouldBeAbleToFilterUsersByDomain(string domain)
    {
        var response = GetStoredResponse();
        var usersResponse = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);

        usersResponse.Should().NotBeNull();

        _filteredUsers = _responseSelector.SelectUsersByEmailDomain(usersResponse!, domain);

        _logger.Information("Selector filtered {Count} users with domain {Domain}",
            _filteredUsers.Count, domain);
    }

    [Then("the filtered users list should contain at least {int} user")]
    public void ThenFilteredUsersListShouldContainAtLeast(int minCount)
    {
        _filteredUsers.Should().NotBeNull();
        _filteredUsers!.Count.Should().BeGreaterThanOrEqualTo(minCount,
            because: $"Should have at least {minCount} matching users");
    }

    [Then("I should be able to select posts by user {int}")]
    public void ThenIShouldBeAbleToSelectPostsByUser(int userId)
    {
        var response = GetStoredResponse();
        var posts = _jsonHelper.Deserialize<List<PostResponse>>(response.Content!);

        posts.Should().NotBeNull();

        _filteredPosts = _responseSelector.SelectPosts(posts!, userId: userId);

        _logger.Information("Selector filtered {Count} posts for user {UserId}",
            _filteredPosts.Count, userId);
    }

    [Then("the selected posts should all belong to user {int}")]
    public void ThenSelectedPostsShouldAllBelongToUser(int userId)
    {
        _filteredPosts.Should().NotBeNull();
        _filteredPosts.Should().NotBeEmpty();

        _filteredPosts!.Should().AllSatisfy(post =>
            post.UserId.Should().Be(userId,
                because: $"All filtered posts should belong to user {userId}"));
    }

    // ═══════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════

    private RestResponse GetStoredResponse()
    {
        _scenarioContext.TryGetValue(ScenarioContextKeys.LastResponse, out RestResponse? response);
        response.Should().NotBeNull(
            because: "A 'When' step must execute before 'Then' assertions");
        return response!;
    }
}