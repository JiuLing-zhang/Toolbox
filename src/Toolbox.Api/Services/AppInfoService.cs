using Toolbox.Api.Interface;

namespace Toolbox.Api.Services;
public class AppInfoService : IAppInfoService
{
    private readonly IAppInfoRepository _appInfoRepository;
    private readonly IAppBaseRepository _appBaseRepository;
    public AppInfoService(IAppInfoRepository appInfoRepository, IAppBaseRepository appBaseRepository)
    {
        _appInfoRepository = appInfoRepository;
        _appBaseRepository = appBaseRepository;
    }
    public async Task<(string FilePath, string ContentType)> GetDownloadInfoAsync(string id)
    {
        var appInfo = await _appInfoRepository.GetOneAsync(id);
        if (appInfo == null)
        {
            return ("", "");
        }

        await _appBaseRepository.DownloadOnceAsync(appInfo.AppKey);
        return (appInfo.FilePath, appInfo.ContentType);
    }
}