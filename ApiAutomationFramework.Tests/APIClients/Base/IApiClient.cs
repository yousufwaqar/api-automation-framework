using RestSharp;

namespace ApiAutomationFramework.APIClients.Base;

public interface IApiClient
{
    Task<RestResponse> ExecuteAsync(RestRequest request);
    Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) where T : class;
    RestRequest CreateRequest(string endpoint, Method method);
}