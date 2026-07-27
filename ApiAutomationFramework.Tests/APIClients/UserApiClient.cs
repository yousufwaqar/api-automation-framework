using ApiAutomationFramework.APIClients.Base;
using ApiAutomationFramework.APIClients.Interfaces;
using ApiAutomationFramework.Configuration;
using ApiAutomationFramework.Constants;
using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using ApiAutomationFramework.Helpers;
using RestSharp;

namespace ApiAutomationFramework.APIClients;

public class UserApiClient : BaseApiClient, IUserApiClient
{
    public UserApiClient(AppSettings settings, RetryHelper retryHelper)
        : base(settings.ApiSettings.ReqRes, settings, retryHelper)
    {
    }

    public async Task<RestResponse<UsersListResponse>> GetUsersAsync(int page = 1)
    {
        var request = CreateRequest(ApiEndpoints.Users.GetAll, Method.Get);
        request.AddQueryParameter("page", page.ToString());
        return await ExecuteAsync<UsersListResponse>(request);
    }

    public async Task<RestResponse<UserResponse>> GetUserAsync(int userId)
    {
        var request = CreateRequest(ApiEndpoints.Users.GetById, Method.Get);
        request.AddUrlSegment("id", userId.ToString());
        return await ExecuteAsync<UserResponse>(request);
    }

    public async Task<RestResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest createRequest)
    {
        var request = CreateRequest(ApiEndpoints.Users.Create, Method.Post);
        request.AddJsonBody(createRequest);
        return await ExecuteAsync<CreateUserResponse>(request);
    }

    public async Task<RestResponse<CreateUserResponse>> UpdateUserAsync(
        int userId, UpdateUserRequest updateRequest)
    {
        var request = CreateRequest(ApiEndpoints.Users.Update, Method.Put);
        request.AddUrlSegment("id", userId.ToString());
        request.AddJsonBody(updateRequest);
        return await ExecuteAsync<CreateUserResponse>(request);
    }

    public async Task<RestResponse<CreateUserResponse>> PatchUserAsync(
        int userId, UpdateUserRequest patchRequest)
    {
        var request = CreateRequest(ApiEndpoints.Users.Update, Method.Patch);
        request.AddUrlSegment("id", userId.ToString());
        request.AddJsonBody(patchRequest);
        return await ExecuteAsync<CreateUserResponse>(request);
    }

    public async Task<RestResponse> DeleteUserAsync(int userId)
    {
        var request = CreateRequest(ApiEndpoints.Users.Delete, Method.Delete);
        request.AddUrlSegment("id", userId.ToString());
        return await ExecuteAsync(request);
    }

    public async Task<RestResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var request = CreateRequest(ApiEndpoints.Authentication.Login, Method.Post);
        request.AddJsonBody(loginRequest);
        return await ExecuteAsync<LoginResponse>(request);
    }
}