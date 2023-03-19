using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toolbox.Api.Entities;
[Table("AppInfo")]
public class AppInfo
{
    /// <summary>
    /// 文件名的MD5
    /// </summary>
    [Key]
    public string Id { get; set; } = null!;
    public DateTime CreateTime { get; set; }
    public string AppKey { get; set; } = null!;
    public string Platform { get; set; } = null!;
    public int VersionCode { get; set; }
    public string VersionName { get; set; } = null!;
    public string MinVersionName { get; set; } = null!;
    public string? UpgradeLog { get; set; }
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string SignType { get; set; } = null!;
    public string SignValue { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public int FileLength { get; set; }
}