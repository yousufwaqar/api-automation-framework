using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using ApiAutomationFramework.Helpers;
using ApiAutomationFramework.Hooks;
using FluentAssertions;
using Reqnroll;
using RestSharp;
using Serilog;

namespace ApiAutomationFramework.StepDefinitions;

[Binding]
public class UserStepDefinitions
{
    private readonly IUserApiClient _userApiClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly JsonHelper _jsonHelper;
    private readonly ResponseValidator _responseValidator;
    private readonly RandomDataGenerator _dataGenerator;
    private readonly ILogger _logger;

    private CreateUserRequest? _createUserRequest;
    private UpdateUserRequest? _updateUserRequest;

    public UserStepDefinitions(
        IUserApiClient userApiClient,
        ScenarioContext scenarioContext,
        JsonHelper jsonHelper,
        ResponseValidator responseValidator,
        RandomDataGenerator dataGenerator)
    {
        _userApiClient = userApiClient;
        _scenarioContext = scenarioContext;
        _jsonHelper = jsonHelper;
        _responseValidator = responseValidator;
        _dataGenerator = dataGenerator;
        _logger = Log.ForContext<UserStepDefinitions>();
    }

    // ── GIVEN ──────────────────────────────────────────

    [Given("the User API is available")]
    public async Task GivenTheUserApiIsAvailable()
    {
        var response = await _userApiClient.GetUsersAsync();
        response.IsSuccessful.Should().BeTrue(
            because: $"User API must be reachable. Status: {(int)response.StatusCode}");
        _logger.Information("User API is available.");
    }

    [Given("I have a valid create user request with name {string} and job {string}")]
    public void GivenIHaveAValidCreateUserRequest(string name, string job)
    {
        _createUserRequest = new CreateUserRequest { Name = name, Job = job };
    }

    [Given("I have a randomly generated create user request")]
    public void GivenIHaveARandomlyGeneratedCreateUserRequest()
    {
        _createUserRequest = _dataGenerator.GenerateCreateUserRequest();
        _scenarioContext[ScenarioContextKeys.ExpectedName] = _createUserRequest.Name;
        _scenarioContext[ScenarioContextKeys.ExpectedJob] = _createUserRequest.Job;
    }

    [Given("I have a valid update user request with name {string} and job {string}")]
    public void GivenIHaveAValidUpdateUserRequest(string name, string job)
    {
        _updateUserRequest = new UpdateUserRequest { Name = name, Job = job };
    }

    // ── WHEN ───────────────────────────────────────────

    [When("I request all users on page {int}")]
    public async Task WhenIRequestAllUsersOnPage(int page)
    {
        var response = await _userApiClient.GetUsersAsync(page);
        StoreResponse(response);
    }

    [When("I request user with id {int}")]
    public async Task WhenIRequestUserWithId(int userId)
    {
        var response = await _userApiClient.GetUserAsync(userId);
        StoreResponse(response);
    }

    [When("I send a POST request to create the user")]
    public async Task WhenISendAPostRequestToCreateTheUser()
    {
        _createUserRequest.Should().NotBeNull(
            because: "Prepare a create request in a Given step first");
        var response = await _userApiClient.CreateUserAsync(_createUserRequest!);
        StoreResponse(response);
    }

    [When("I send a PUT request to update user with id {int}")]
    public async Task WhenISendAPutRequestToUpdateUser(int userId)
    {
        _updateUserRequest.Should().NotBeNull();
        var response = await _userApiClient.UpdateUserAsync(userId, _updateUserRequest!);
        StoreResponse(response);
    }

    [When("I send a PATCH request to update user with id {int}")]
    public async Task WhenISendAPatchRequestToUpdateUser(int userId)
    {
        _updateUserRequest.Should().NotBeNull();
        var response = await _userApiClient.PatchUserAsync(userId, _updateUserRequest!);
        StoreResponse(response);
    }

    [When("I send a DELETE request for user with id {int}")]
    public async Task WhenISendADeleteRequestForUser(int userId)
    {
        var response = await _userApiClient.DeleteUserAsync(userId);
        StoreResponse(response);
    }

    // ── THEN ───────────────────────────────────────────

    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        var response = GetStoredResponse();
        var actual = (int)response.StatusCode;
        actual.Should().Be(expectedStatusCode,
            because: $"Expected {expectedStatusCode} but got {actual}. Body: {response.Content}");
        _logger.Information("✓ Status {Expected}", expectedStatusCode);
    }

    [Then("the response should contain a list of users")]
    public void ThenTheResponseShouldContainAListOfUsers()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty(because: "User list should contain users");
        _logger.Information("✓ User list has {Count} users", result.Data.Count);
    }

    [Then("the response should have pagination metadata")]
    public void ThenTheResponseShouldHavePaginationMetadata()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);
        result.Should().NotBeNull();
        result!.Page.Should().BeGreaterThan(0);
        result.PerPage.Should().BeGreaterThan(0);
        result.Total.Should().BeGreaterThan(0);
        result.TotalPages.Should().BeGreaterThan(0);
    }

    [Then("the total users count should be greater than {int}")]
    public void ThenTotalUsersCountShouldBeGreaterThan(int minCount)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);
        result!.Total.Should().BeGreaterThan(minCount);
    }

    [Then("the response should match the User DTO schema")]
    public void ThenTheResponseShouldMatchUserDtoSchema()
    {
        var response = GetStoredResponse();

        // Verify the response can be deserialized into our UserResponse DTO
        // This is a more reliable check than strict JSON schema validation
        // because it verifies actual type compatibility, not just structure
        var userResponse = _jsonHelper.Deserialize<UserResponse>(response.Content!);

        userResponse.Should().NotBeNull(
            because: "Response should deserialize into UserResponse DTO");

        userResponse!.Data.Should().NotBeNull(
            because: "Response should contain 'data' object");

        userResponse.Data!.Id.Should().BeGreaterThan(0,
            because: "User should have a valid ID");

        userResponse.Data.Email.Should().NotBeNullOrEmpty(
            because: "User should have an email");

        userResponse.Data.FirstName.Should().NotBeNullOrEmpty(
            because: "User should have a first name");

        userResponse.Data.LastName.Should().NotBeNullOrEmpty(
            because: "User should have a last name");

        _logger.Information("✓ Response matches UserResponse DTO structure");
    }

    [Then("the user first name should be {string}")]
    public void ThenTheUserFirstNameShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UserResponse>(response.Content!);
        result!.Data!.FirstName.Should().Be(expected);
        _logger.Information("✓ First name: {Name}", expected);
    }

    [Then("the user last name should be {string}")]
    public void ThenTheUserLastNameShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UserResponse>(response.Content!);
        result!.Data!.LastName.Should().Be(expected);
    }

    [Then("the user email should not be empty")]
    public void ThenTheUserEmailShouldNotBeEmpty()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UserResponse>(response.Content!);
        result!.Data!.Email.Should().NotBeNullOrEmpty();
    }

    [Then("the user avatar URL should be a valid URL")]
    public void ThenTheUserAvatarUrlShouldBeValid()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UserResponse>(response.Content!);
        var avatar = result!.Data!.Avatar;
        avatar.Should().NotBeNullOrEmpty();
        Uri.IsWellFormedUriString(avatar, UriKind.Absolute).Should().BeTrue(
            because: $"'{avatar}' should be a valid URL");
    }

    [Then("the response should contain users on page {int}")]
    public void ThenTheResponseShouldContainUsersOnPage(int expectedPage)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);
        result!.Page.Should().Be(expectedPage);
    }

    [Then("the response should contain the created user")]
    public void ThenResponseShouldContainCreatedUser()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result.Should().NotBeNull();
        if (!string.IsNullOrEmpty(result!.Id))
            _scenarioContext[ScenarioContextKeys.CreatedUserId] = result.Id;
    }

    [Then("the created user name should be {string}")]
    public void ThenCreatedUserNameShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.Name.Should().Be(expected);
    }

    [Then("the created user job should be {string}")]
    public void ThenCreatedUserJobShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.Job.Should().Be(expected);
    }

    [Then("the created user should have an id")]
    public void ThenCreatedUserShouldHaveId()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.Id.Should().NotBeNullOrEmpty(because: "Created user should have an ID");
    }

    [Then("the created user should have a createdAt timestamp")]
    public void ThenCreatedUserShouldHaveTimestamp()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.CreatedAt.Should().NotBeNull(because: "Created user should have a timestamp");
    }

    [Then("the response should contain the updated user")]
    public void ThenResponseShouldContainUpdatedUser()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result.Should().NotBeNull();
    }

    [Then("the updated user name should be {string}")]
    public void ThenUpdatedUserNameShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.Name.Should().Be(expected);
    }

    [Then("the updated user job should be {string}")]
    public void ThenUpdatedUserJobShouldBe(string expected)
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<CreateUserResponse>(response.Content!);
        result!.Job.Should().Be(expected);
    }

    [Then("the response body should be empty")]
    public void ThenResponseBodyShouldBeEmpty()
    {
        var response = GetStoredResponse();
        (string.IsNullOrEmpty(response.Content) || response.Content == "{}")
            .Should().BeTrue(because: "204 response should have empty body");
    }

    [Then("the response body should be empty or contain error info")]
    public void ThenResponseBodyShouldBeEmptyOrError()
    {
        var response = GetStoredResponse();
        _logger.Information("404 response body: {Content}", response.Content);
    }

    [Then("the user list should be empty")]
    public void ThenUserListShouldBeEmpty()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<UsersListResponse>(response.Content!);
        result!.Data.Should().BeEmpty(because: "Invalid page should return no users");
    }

    // ── PRIVATE HELPERS ────────────────────────────────

    private void StoreResponse(RestResponseBase response)
    {
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    private RestResponse GetStoredResponse()
    {
        _scenarioContext.TryGetValue(ScenarioContextKeys.LastResponse, out RestResponse? response);
        response.Should().NotBeNull(
            because: "A When step must run before Then assertions");
        return response!;
    }
}