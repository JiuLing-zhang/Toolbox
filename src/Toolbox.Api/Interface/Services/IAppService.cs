using Toolbox.Api.Enums;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Interface.Services;
public interface IAppService
{
    Task<Dictionary<string, string>> GetAppNamesAsync();
    Task<bool> AllowPublishAsync(string appKey, PlatformEnum platform, string versionName);
    Task<bool> PublishAsync(AppReleaseModel model);
    Task<List<AppInfoResponse>> GetAppsAsync();
    Task<List<ComponentInfoResponse>> GetComponentsAsync();
    Task<(string FilePath, string ContentType)> GetDownloadInfoAsync(string id);
}