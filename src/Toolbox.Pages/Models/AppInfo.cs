using Toolbox.Pages.Enums;

namespace Toolbox.Pages.Models;
internal class AppInfo
{
    /// <summary>
    /// 文件名
    /// </summary> 
    public string AppName { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHub { get; set; }
    public int DownloadCount { get; set; }
    public List<AppVersionInfo>? Versions { get; set; }
}
internal class AppVersionInfo
{
    public PlatformEnum PlatformType { get; set; }
    public string VersionName { get; set; } = null!;
    public DateTime CreateTime { get; set; }
    public string PublishTime => CreateTime.ToString("yyyy-MM-dd");
    public string DownloadId { get; set; } = null!;
    public SignTypeEnum SignType { get; set; }
    public string SignValue { get; set; } = null!;
    public int FileLength { get; set; }
    public string FileLengthMb => FileLength == 0 ? "未知" : $"{((double)FileLength / 1024 / 1024).ToString("0.00")} MB";
}