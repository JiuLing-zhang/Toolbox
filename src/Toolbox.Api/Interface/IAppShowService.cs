using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Interface;
public interface IAppShowService
{
    Task<List<AppInfoResponse>> GetAppsAsync();
}