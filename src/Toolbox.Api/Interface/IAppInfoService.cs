namespace Toolbox.Api.Interface;
public interface IAppInfoService
{
    Task<(string FilePath, string ContentType)> GetDownloadInfoAsync(string id);
}