using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toolbox.Api.Entities;
[Table("AppBase")]
public class AppBase
{
    /// <summary>
    /// 关键字
    /// </summary>
    [Key]
    public string AppKey { get; set; } = null!;
    /// <summary>
    /// 文件的中文名
    /// </summary>
    public string AppName { get; set; } = null!;
    public int Sort { get; set; }
    public string Description { get; set; } = null!;
    public string GitHub { get; set; } = null!;
    public int DownloadCount { get; set; }
    /// <summary>
    /// 图标的URL
    /// </summary>
    public string Icon { get; set; } = null!;
    public bool IsShow { get; set; }

    public string AppKey2 { get; set; } = null!;
}