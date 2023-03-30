namespace Toolbox.Api.Models.Response;
public class AppReleaseResponse
{
    public DateTime CreateTime { get; set; }
    public string AppKey { get; set; } = null!;
    public string Platform { get; set; } = null!;
    public int VersionCode { get; set; }
    public string VersionName { get; set; } = null!;
    public string MinVersionName { get; set; } = null!;
    public string UpgradeLog { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string SignType { get; set; } = null!;
    public string SignValue { get; set; } = null!;
    public int FileLength { get; set; }
}