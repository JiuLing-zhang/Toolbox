using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Interface;
public interface IAppShowService
{
    Task<Dictionary<string,string>> GetAppNamesAsync();
    Task<List<AppInfoResponse>> GetAppsAsync();
    Task<List<ComponentInfoResponse>> GetComponentsAsync();
}