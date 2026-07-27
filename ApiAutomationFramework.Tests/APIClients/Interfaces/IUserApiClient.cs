using ApiAutomationFramework.DTOs.Request;
using ApiAutomationFramework.DTOs.Response;
using RestSharp;

namespace ApiAutomationFramework.APIClients.Interfaces;

public interface IUserApiClient
{
    Task<RestResponse<UsersListResponse>> GetUsersAsync(int page = 1);
    Task<RestResponse<UserResponse>> GetUserAsync(int userId);
    Task<RestResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
    Task<RestResponse<CreateUserResponse>> UpdateUserAsync(int userId, UpdateUserRequest request);
    Task<RestResponse<CreateUserResponse>> PatchUserAsync(int userId, UpdateUserRequest request);
    Task<RestResponse> DeleteUserAsync(int userId);
    Task<RestResponse<LoginResponse>> LoginAsync(LoginRequest request);
}