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
public class AuthenticationStepDefinitions
{
    private readonly IUserApiClient _userApiClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly TokenGenerator _tokenGenerator;
    private readonly JsonHelper _jsonHelper;
    private readonly ILogger _logger;

    private LoginRequest? _loginRequest;

    public AuthenticationStepDefinitions(
        IUserApiClient userApiClient,
        ScenarioContext scenarioContext,
        TokenGenerator tokenGenerator,
        JsonHelper jsonHelper)
    {
        _userApiClient = userApiClient;
        _scenarioContext = scenarioContext;
        _tokenGenerator = tokenGenerator;
        _jsonHelper = jsonHelper;
        _logger = Log.ForContext<AuthenticationStepDefinitions>();
    }

    [Given("I have valid login credentials")]
    public void GivenIHaveValidLoginCredentials()
    {
        _loginRequest = LoginRequest.ValidCredentials();
    }

    [Given("I have invalid login credentials")]
    public void GivenIHaveInvalidLoginCredentials()
    {
        _loginRequest = LoginRequest.InvalidCredentials();
    }

    [Given("I have login credentials with missing password")]
    public void GivenIHaveLoginCredentialsWithMissingPassword()
    {
        _loginRequest = LoginRequest.MissingPassword();
    }

    [Given("I successfully login with valid credentials")]
    public async Task GivenISuccessfullyLoginWithValidCredentials()
    {
        _loginRequest = LoginRequest.ValidCredentials();
        var response = await _userApiClient.LoginAsync(_loginRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            because: "Login must succeed to test authenticated endpoints");

        var loginResponse = _jsonHelper.Deserialize<LoginResponse>(response.Content!);
        loginResponse!.Token.Should().NotBeNullOrEmpty();
        _tokenGenerator.StoreLoginToken(loginResponse);
        _scenarioContext[ScenarioContextKeys.AuthToken] = loginResponse.Token;
    }

    [Given("I have the authentication token")]
    public void GivenIHaveTheAuthenticationToken()
    {
        _tokenGenerator.HasValidToken().Should().BeTrue(
            because: "Login step must complete successfully first");
    }

    [When("I send a login request")]
    public async Task WhenISendALoginRequest()
    {
        _loginRequest.Should().NotBeNull();
        var response = await _userApiClient.LoginAsync(_loginRequest!);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [When("I request users with the authentication token")]
    public async Task WhenIRequestUsersWithAuthToken()
    {
        var response = await _userApiClient.GetUsersAsync();
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [Then("the response should contain an authentication token")]
    public void ThenResponseShouldContainToken()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<LoginResponse>(response.Content!);
        result.Should().NotBeNull();
        result!.IsSuccessful.Should().BeTrue(because: "Login should return a token");
    }

    [Then("the token should not be empty")]
    public void ThenTokenShouldNotBeEmpty()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<LoginResponse>(response.Content!);
        result!.Token.Should().NotBeNullOrWhiteSpace();
        _logger.Information("✓ Token received, length: {Len}", result.Token!.Length);
    }

    [Then("the response should contain an error message")]
    public void ThenResponseShouldContainErrorMessage()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<ErrorResponse>(response.Content!);
        result!.Error.Should().NotBeNullOrEmpty(
            because: "Error response should explain why login failed");
    }

    [Then("the error message should indicate missing password")]
    public void ThenErrorMessageShouldIndicateMissingPassword()
    {
        var response = GetStoredResponse();
        var result = _jsonHelper.Deserialize<ErrorResponse>(response.Content!);
        result!.Error.Should().NotBeNullOrEmpty();
        result.Error!.ToLower().Should().ContainAny(
            new[] { "password", "missing", "required" },
            because: "Error should mention missing password");
    }

    private RestResponse GetStoredResponse()
    {
        _scenarioContext.TryGetValue(ScenarioContextKeys.LastResponse, out RestResponse? response);
        response.Should().NotBeNull();
        return response!;
    }
}