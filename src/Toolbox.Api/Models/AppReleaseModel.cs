using Toolbox.Api.Enums;

namespace Toolbox.Api.Models;
public class AppReleaseModel
{
    public string AppKey { get; set; }
    public PlatformEnum Platform { get; set; }
    public string VersionName { get; set; }
    public bool IsMinVersion { get; set; }
    public string UpgradeLog { get; set; }
    public string FilePath { get; set; }
    public int FileLength { get; set; }
    public SignTypeEnum SignType { get; set; }
    public string SignValue { get; set; }

    public AppReleaseModel(string appKey, PlatformEnum platform, string versionName, bool isMinVersion, string upgradeLog, string filePath, int fileLength, SignTypeEnum signType, string signValue)
    {
        AppKey = appKey;
        Platform = platform;
        VersionName = versionName;
        IsMinVersion = isMinVersion;
        UpgradeLog = upgradeLog;
        FilePath = filePath;
        FileLength = fileLength;
        SignType = signType;
        SignValue = signValue;
    }
}