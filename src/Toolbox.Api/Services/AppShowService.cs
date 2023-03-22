using Toolbox.Api.Entities;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface;
using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Services;
internal class AppShowService : IAppShowService
{
    private readonly IAppBaseRepository _appBaseRepository;
    private readonly IAppInfoRepository _appInfoRepository;
    private readonly IComponentRepository _componentRepository;

    public AppShowService(IAppBaseRepository appBaseRepository, IAppInfoRepository appInfoRepository, IComponentRepository componentRepository)
    {
        _appBaseRepository = appBaseRepository;
        _appInfoRepository = appInfoRepository;
        _componentRepository = componentRepository;
    }

    public async Task<List<AppInfoResponse>> GetAppsAsync()
    {
        var result = new List<AppInfoResponse>();

        var appBaseList = await _appBaseRepository.GetAllAsync();
        var appInfoList = await _appInfoRepository.GetAllAsync();
        foreach (var appBase in appBaseList)
        {
            var resultItem = new AppInfoResponse()
            {
                AppName = appBase.AppName,
                Icon = appBase.Icon,
                Description = appBase.Description,
                DownloadCount = appBase.DownloadCount,
                GitHub = appBase.GitHub
            };

            if (!appBase.IsShow)
            {
                result.Add(resultItem);
                continue;
            }

            string appKey = appBase.AppKey;

            IEnumerable<AppInfo> appVersions = appInfoList.Where(x => x.AppKey == appKey).ToList();

            List<AppVersionInfoResponse> versions = new List<AppVersionInfoResponse>();
            BuildAppInfo(appVersions, PlatformEnum.Windows, out var windowsVersion);
            if (windowsVersion != null)
            {
                versions.Add(windowsVersion);
            }

            BuildAppInfo(appVersions, PlatformEnum.Android, out var androidVersion);
            if (androidVersion != null)
            {
                versions.Add(androidVersion);
            }

            BuildAppInfo(appVersions, PlatformEnum.IOS, out var iosVersion);
            if (iosVersion != null)
            {
                versions.Add(iosVersion);
            }

            if (versions.Count > 0)
            {
                resultItem.Versions = versions;
            }

            result.Add(resultItem);
        }

        return result;
    }

    public async Task<List<ComponentInfoResponse>> GetComponentsAsync()
    {
        var components = await _componentRepository.GetAllAsync();
        return components.Select(x => new ComponentInfoResponse
        {
            Name = x.Name,
            Icon = x.Icon,
            Description = x.Description,
            GitHub = x.GitHub,
        }).ToList();
    }

    private void BuildAppInfo(IEnumerable<AppInfo> apps, PlatformEnum platform, out AppVersionInfoResponse? versions)
    {
        var platformInfo = apps.Where(x => x.Platform == platform.ToString()).MaxBy(x => x.CreateTime);
        if (platformInfo == null)
        {
            versions = null;
            return;
        }

        versions = new AppVersionInfoResponse()
        {
            CreateTime = platformInfo.CreateTime,
            DownloadId = platformInfo.Id,
            PlatformType = platform,
            VersionName = platformInfo.VersionName,
            SignType = (SignTypeEnum)Enum.Parse(typeof(SignTypeEnum), platformInfo.SignType),
            SignValue = platformInfo.SignValue,
            FileLength = platformInfo.FileLength,
        };
    }
}