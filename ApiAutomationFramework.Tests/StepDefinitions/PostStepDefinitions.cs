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
public class PostStepDefinitions
{
    private readonly IPostApiClient _postApiClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly JsonHelper _jsonHelper;
    private readonly RandomDataGenerator _dataGenerator;
    private readonly ILogger _logger;

    private CreatePostRequest? _createPostRequest;

    public PostStepDefinitions(
        IPostApiClient postApiClient,
        ScenarioContext scenarioContext,
        JsonHelper jsonHelper,
        RandomDataGenerator dataGenerator)
    {
        _postApiClient = postApiClient;
        _scenarioContext = scenarioContext;
        _jsonHelper = jsonHelper;
        _dataGenerator = dataGenerator;
        _logger = Log.ForContext<PostStepDefinitions>();
    }

    [Given("the Posts API is available")]
    public async Task GivenThePostsApiIsAvailable()
    {
        var response = await _postApiClient.GetPostsAsync();
        response.IsSuccessful.Should().BeTrue(
            because: $"Posts API must be reachable. Status: {(int)response.StatusCode}");
    }

    [Given("I have a valid create post request")]
    public void GivenIHaveAValidCreatePostRequest()
    {
        _createPostRequest = _dataGenerator.GenerateCreatePostRequest();
        _scenarioContext[ScenarioContextKeys.ExpectedPostTitle] = _createPostRequest.Title;
    }

    [Given("I have a valid update post request")]
    public void GivenIHaveAValidUpdatePostRequest()
    {
        _createPostRequest = _dataGenerator.GenerateCreatePostRequest();
    }

    [When("I request all posts")]
    public async Task WhenIRequestAllPosts()
    {
        var response = await _postApiClient.GetPostsAsync();
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [When("I request post with id {int}")]
    public async Task WhenIRequestPostWithId(int postId)
    {
        var response = await _postApiClient.GetPostAsync(postId);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [When("I request posts by user with id {int}")]
    public async Task WhenIRequestPostsByUserId(int userId)
    {
        var response = await _postApiClient.GetPostsByUserAsync(userId);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
        _scenarioContext[ScenarioContextKeys.RequestedUserId] = userId;
    }

    [When("I send a POST request to create the post")]
    public async Task WhenISendAPostRequestToCreatePost()
    {
        _createPostRequest.Should().NotBeNull();
        var response = await _postApiClient.CreatePostAsync(_createPostRequest!);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [When("I send a PUT request to update post with id {int}")]
    public async Task WhenISendAPutRequestToUpdatePost(int postId)
    {
        _createPostRequest.Should().NotBeNull();
        var response = await _postApiClient.UpdatePostAsync(postId, _createPostRequest!);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [When("I send a DELETE request for post with id {int}")]
    public async Task WhenISendADeleteRequestForPost(int postId)
    {
        var response = await _postApiClient.DeletePostAsync(postId);
        _scenarioContext[ScenarioContextKeys.LastResponse] = response;
    }

    [Then("the response should contain a list of posts")]
    public void ThenResponseShouldContainListOfPosts()
    {
        var response = GetStoredResponse();
        var posts = _jsonHelper.Deserialize<List<PostResponse>>(response.Content!);
        posts.Should().NotBeNull();
        posts.Should().NotBeEmpty();
    }

    [Then("the posts list should have {int} items")]
    public void ThenPostsListShouldHaveItems(int expectedCount)
    {
        var response = GetStoredResponse();
        var posts = _jsonHelper.Deserialize<List<PostResponse>>(response.Content!);
        posts!.Count.Should().Be(expectedCount,
            because: $"JSONPlaceholder /posts returns exactly {expectedCount} posts");
        _logger.Information("✓ Posts count: {Count}", posts.Count);
    }

    [Then("the post should have a valid id")]
    public void ThenPostShouldHaveValidId()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);
        post!.Id.Should().BeGreaterThan(0);
    }

    [Then("the post title should not be empty")]
    public void ThenPostTitleShouldNotBeEmpty()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);
        post!.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Then("the post body should not be empty")]
    public void ThenPostBodyShouldNotBeEmpty()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);
        post!.Body.Should().NotBeNullOrWhiteSpace();
    }

    [Then("the post should belong to a user")]
    public void ThenPostShouldBelongToUser()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);
        post!.UserId.Should().BeGreaterThan(0);
    }

    [Then("all returned posts should belong to user {int}")]
    public void ThenAllPostsShouldBelongToUser(int userId)
    {
        var response = GetStoredResponse();
        var posts = _jsonHelper.Deserialize<List<PostResponse>>(response.Content!);
        posts.Should().NotBeNull();
        posts!.Should().AllSatisfy(p =>
            p.UserId.Should().Be(userId,
                because: $"All posts should belong to user {userId}"));
    }

    [Then("the posts list should not be empty")]
    public void ThenPostsListShouldNotBeEmpty()
    {
        var response = GetStoredResponse();
        var posts = _jsonHelper.Deserialize<List<PostResponse>>(response.Content!);
        posts.Should().NotBeEmpty();
    }

    [Then("the created post should have an id")]
    public void ThenCreatedPostShouldHaveId()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);
        post!.Id.Should().BeGreaterThan(0);
        _scenarioContext[ScenarioContextKeys.CreatedPostId] = post.Id;
    }

    [Then("the created post title should match the request title")]
    public void ThenCreatedPostTitleShouldMatchRequestTitle()
    {
        var response = GetStoredResponse();
        var post = _jsonHelper.Deserialize<PostResponse>(response.Content!);

        if (_scenarioContext.TryGetValue(ScenarioContextKeys.ExpectedPostTitle, out string? title))
        {
            post!.Title.Should().Be(title);
        }
    }

    private RestResponse GetStoredResponse()
    {
        _scenarioContext.TryGetValue(ScenarioContextKeys.LastResponse, out RestResponse? response);
        response.Should().NotBeNull();
        return response!;
    }
}