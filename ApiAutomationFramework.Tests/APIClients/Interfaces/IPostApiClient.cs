using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using RestSharp;

namespace ApiAutomationFramework.APIClients.Interfaces;

public interface IPostApiClient
{
    Task<RestResponse<List<PostResponse>>> GetPostsAsync();
    Task<RestResponse<PostResponse>> GetPostAsync(int postId);
    Task<RestResponse<List<PostResponse>>> GetPostsByUserAsync(int userId);
    Task<RestResponse<PostResponse>> CreatePostAsync(CreatePostRequest request);
    Task<RestResponse<PostResponse>> UpdatePostAsync(int postId, CreatePostRequest request);
    Task<RestResponse> DeletePostAsync(int postId);
}