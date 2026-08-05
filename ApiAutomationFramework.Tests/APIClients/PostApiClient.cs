using ApiAutomationFramework.APIClients.Base;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Constants;
using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using ApiAutomationFramework.Helpers;
using RestSharp;

namespace ApiAutomationFramework.APIClients;

public class PostApiClient : BaseApiClient, IPostApiClient
{
    public PostApiClient(AppSettings settings, RetryHelper retryHelper)
        : base(settings.ApiSettings.JsonPlaceholder, settings, retryHelper)
    {
    }

    public async Task<RestResponse<List<PostResponse>>> GetPostsAsync()
    {
        var request = CreateRequest(ApiEndpoints.Posts.GetAll, Method.Get);
        return await ExecuteAsync<List<PostResponse>>(request);
    }

    public async Task<RestResponse<PostResponse>> GetPostAsync(int postId)
    {
        var request = CreateRequest(ApiEndpoints.Posts.GetById, Method.Get);
        request.AddUrlSegment("id", postId.ToString());
        return await ExecuteAsync<PostResponse>(request);
    }

    public async Task<RestResponse<List<PostResponse>>> GetPostsByUserAsync(int userId)
    {
        var request = CreateRequest(ApiEndpoints.Posts.GetAll, Method.Get);
        request.AddQueryParameter("userId", userId.ToString());
        return await ExecuteAsync<List<PostResponse>>(request);
    }

    public async Task<RestResponse<PostResponse>> CreatePostAsync(CreatePostRequest createRequest)
    {
        var request = CreateRequest(ApiEndpoints.Posts.Create, Method.Post);
        request.AddJsonBody(createRequest);
        return await ExecuteAsync<PostResponse>(request);
    }

    public async Task<RestResponse<PostResponse>> UpdatePostAsync(
        int postId, CreatePostRequest updateRequest)
    {
        var request = CreateRequest(ApiEndpoints.Posts.Update, Method.Put);
        request.AddUrlSegment("id", postId.ToString());
        request.AddJsonBody(updateRequest);
        return await ExecuteAsync<PostResponse>(request);
    }

    public async Task<RestResponse> DeletePostAsync(int postId)
    {
        var request = CreateRequest(ApiEndpoints.Posts.Delete, Method.Delete);
        request.AddUrlSegment("id", postId.ToString());
        return await ExecuteAsync(request);
    }
}