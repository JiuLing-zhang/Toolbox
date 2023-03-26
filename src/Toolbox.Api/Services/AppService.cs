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
    public AppService(IAppReleaseRepository appReleaseRepository, IAppBaseRepository appBaseRepository)
    {
        _appReleaseRepository = appReleaseRepository;
        _appBaseRepository = appBaseRepository;
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
    public Task<List<AppInfoResponse>> GetAppsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<ComponentInfoResponse>> GetComponentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<(string FilePath, string ContentType)> GetDownloadInfoAsync(string id)
    {
        throw new NotImplementedException();
    }

}