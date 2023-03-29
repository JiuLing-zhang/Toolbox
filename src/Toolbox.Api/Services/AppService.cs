using JiuLing.CommonLibs;
using Toolbox.Api.Entities;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface;
using Toolbox.Api.Interface.Repositories;
using Toolbox.Api.Interface.Services;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Services;
public class AppService : IAppService
{
    private readonly IAppBaseRepository _appBaseRepository;
    private readonly IAppReleaseRepository _appReleaseRepository;
    private readonly IComponentRepository _componentRepository;
    public AppService(IAppReleaseRepository appReleaseRepository, IAppBaseRepository appBaseRepository, IComponentRepository componentRepository)
    {
        _appReleaseRepository = appReleaseRepository;
        _appBaseRepository = appBaseRepository;
        _componentRepository = componentRepository;
    }
    public async Task<Dictionary<string, string>> GetAppNamesAsync()
    {
        var appBaseList = await _appBaseRepository.GetAllAsync();
        return appBaseList.ToDictionary(x => x.AppKey, x => x.AppName);
    }

    public async Task<bool> AllowPublishAsync(string appKey, PlatformEnum platform, string versionName)
    {
        if (!await _appBaseRepository.ExistAsync(appKey))
        {
            return false;
        }
        var release = await _appReleaseRepository.GetLastVersionAsync(appKey, platform);
        if (release == null)
        {
            return true;
        }

        if (!VersionUtils.CheckNeedUpdate(release.VersionName, versionName))
        {
            return false;
        }
        return true;
    }

    public async Task<bool> PublishAsync(AppReleaseModel dto)
    {
        string minVersionName;
        if (dto.IsMinVersion)
        {
            //如果是最小版本，版本号直接取自身
            minVersionName = dto.VersionName;
        }
        else
        {
            var lastVersion = await _appReleaseRepository.GetLastVersionAsync(dto.AppKey, dto.Platform);
            //第一次发布版本时，最小版本号取自身版本
            minVersionName = lastVersion == null ? dto.VersionName : lastVersion.MinVersionName;
        }

        var version = new Version(dto.VersionName);
        var appRelease = new AppRelease()
        {
            AppKey = dto.AppKey,
            Platform = dto.Platform.ToString(),
            VersionCode = version.Revision,
            VersionName = dto.VersionName,
            MinVersionName = minVersionName,
            UpgradeLog = dto.UpgradeLog,
            FilePath = dto.FilePath,
            SignType = dto.SignType.ToString(),
            SignValue = dto.SignValue,
            IsEnabled = true,
            FileLength = dto.FileLength,
            CreateTime = DateTime.Now
        };

        var count = await _appReleaseRepository.AddAsync(appRelease);
        return count > 0;
    }
    public async Task<List<AppInfoResponse>> GetAppsAsync()
    {
        var result = new List<AppInfoResponse>();

        var appBaseList = await _appBaseRepository.GetAllAsync();
        var appInfoList = await _appReleaseRepository.GetAllAsync();
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

            IEnumerable<AppRelease> appVersions = appInfoList.Where(x => x.AppKey == appKey).ToList();

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

            BuildAppInfo(appVersions, PlatformEnum.iOS, out var iosVersion);
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

    private void BuildAppInfo(IEnumerable<AppRelease> apps, PlatformEnum platform, out AppVersionInfoResponse? versions)
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
            DownloadPath = platformInfo.FilePath,
            PlatformType = platform,
            VersionName = platformInfo.VersionName,
            SignType = (SignTypeEnum)Enum.Parse(typeof(SignTypeEnum), platformInfo.SignType),
            SignValue = platformInfo.SignValue,
            FileLength = platformInfo.FileLength,
        };
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

    public Task<(string FilePath, string ContentType)> GetDownloadInfoAsync(string id)
    {
        throw new NotImplementedException();
    }

}