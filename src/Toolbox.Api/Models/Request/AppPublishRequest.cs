using System.ComponentModel.DataAnnotations;
using Toolbox.Api.Enums;

namespace Toolbox.Api.Models.Request;

public class AppPublishRequest
{
    [Required(ErrorMessage = "请选择App")]
    public string AppKey { get; set; } = null!;

    [Required(ErrorMessage = "平台不能为空")]
    public PlatformEnum Platform { get; set; }

    [Required(ErrorMessage = "版本号不能为空")]
    public string VersionName { get; set; } = null!;

    [Required(ErrorMessage = "版本设置不能为空")]
    public bool IsMinVersion { get; set; }

    public string? Log { get; set; }

    [Required(ErrorMessage = "签名方式不能为空")]
    public SignTypeEnum SignType { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { set; get; } = null!;
}