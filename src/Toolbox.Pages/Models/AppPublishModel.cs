using Microsoft.AspNetCore.Components.Forms;
using Toolbox.Pages.Enums;

namespace Toolbox.Pages.Models;
internal class AppPublishModel
{
    public string AppKey { get; set; } = null!;
    public PlatformEnum Platform { get; set; }
    public string VersionName { get; set; } = null!;
    public bool IsMinVersion { get; set; }
    public string Log { get; set; } = null!;
    public SignTypeEnum SignType { get; set; } = SignTypeEnum.SHA1;
    public string Password { set; get; } = null!;
    public string FileName { set; get; } = null!;
    public IBrowserFile File { get; set; } = null!;
}