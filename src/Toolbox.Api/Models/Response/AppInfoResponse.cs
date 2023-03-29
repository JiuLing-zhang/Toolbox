using Toolbox.Api.Enums;

namespace Toolbox.Api.Models.Response;
public class AppInfoResponse
{
    /// <summary>
    /// 文件名
    /// </summary> 
    public string AppName { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHub { get; set; }
    public int DownloadCount { get; set; }
    public List<AppVersionInfoResponse>? Versions { get; set; }
}

public class AppVersionInfoResponse
{
    public PlatformEnum PlatformType { get; set; }
    public string VersionName { get; set; } = null!;
    public DateTime CreateTime { get; set; }
    public string PublishTime => CreateTime.ToString("yyyy-MM-dd");
    public string DownloadPath { get; set; } = null!;
    public SignTypeEnum SignType { get; set; }
    public string SignValue { get; set; } = null!;
    public int FileLength { get; set; }
    public string FileLengthMb => FileLength == 0 ? "未知" : $"{((double)FileLength / 1024 / 1024).ToString("0.00")} MB";
}