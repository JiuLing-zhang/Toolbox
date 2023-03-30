using System.Web;
using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.CommonLibs.Model;
using Microsoft.AspNetCore.Mvc;
using Toolbox.Api.Entities;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface;
using Toolbox.Api.Interface.Services;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Request;
using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Controllers;
[Route("app")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IAppService _appService;
    private readonly IDatabaseConfigService _databaseConfigService;
    public AppController(IHostEnvironment hostEnvironment, IAppService appService, IDatabaseConfigService databaseConfigService)
    {
        _hostEnvironment = hostEnvironment;
        _appService = appService;
        _databaseConfigService = databaseConfigService;
    }

    [HttpGet("app-name-list")]
    public async Task<IActionResult> GetAppNamesAsync()
    {
        var apps = await _appService.GetAppNamesAsync();
        return Ok(new ApiResponse<Dictionary<string, string>>(0, "操作成功", apps));
    }

    [HttpPost("publish")]
    public async Task<IActionResult> PublishAsync([FromForm] AppPublishRequest request)
    {

        if (Request.Form.Files.Count != 1)
        {
            return Ok(new ApiResponse(101, "头像未成功上传"));
        }
        var file = Request.Form.Files[0];
        if (file.Length == 0)
        {
            return Ok(new ApiResponse(102, "文件不能为空"));
        }

        var config = await _databaseConfigService.GetOneAsync<PublishConfig>("publish");
        if (config == null)
        {
            return Ok(new ApiResponse(1, "配置加载失败"));
        }

        if (request.Password != config.Password)
        {
            return Ok(new ApiResponse(2, "密码错误"));
        }

        if (!await _appService.AllowPublishAsync(request.AppKey, request.Platform, request.VersionName))
        {
            return Ok(new ApiResponse(3, "版本号不能小于历史版本"));
        }

        var random = JiuLing.CommonLibs.RandomUtils.GetOneByLength(4);
        var fileName = $"{request.AppKey}_{request.Platform}_{request.VersionName}_{random}".ToLower();

        var fileExtension = Path.GetExtension(Path.GetFileName(file.FileName));
        fileName = $"{fileName}{fileExtension}";

        var relativePath = $"{GlobalConfig.AppFolder}/{fileName}";
        var directory = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", GlobalConfig.AppFolder);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var filePath = Path.Combine(directory, fileName);
        var signValue = "";
        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
            switch (request.SignType)
            {
                case SignTypeEnum.None:
                    break;
                case SignTypeEnum.MD5:
                    signValue = JiuLing.CommonLibs.Security.MD5Utils.GetFileValueToLower(stream);
                    break;
                case SignTypeEnum.SHA1:
                    signValue = JiuLing.CommonLibs.Security.SHA1Utils.GetFileValueToLower(stream);
                    break;
                default:
                    return Ok(new ApiResponse(4, "错误的签名方式"));
            }
        }

        var model = new AppReleaseModel(
            request.AppKey,
            request.Platform,
            request.VersionName,
            request.IsMinVersion,
            request.Log ?? "",
            relativePath,
            (int)file.Length,
            request.SignType,
            signValue);
        var result = await _appService.PublishAsync(model);
        if (result)
        {
            return Ok(new ApiResponse(0, "发布成功"));
        }
        return Ok(new ApiResponse(1, "发布失败"));
    }

    [HttpGet("app-list")]
    public async Task<IActionResult> GetAppsAsync()
    {
        var apps = await _appService.GetAppsAsync();
        return Ok(new ApiResponse<List<AppInfoResponse>>(0, "操作成功", apps));
    }

    [HttpGet("component-list")]
    public async Task<IActionResult> GetComponentsAsync()
    {
        var components = await _appService.GetComponentsAsync();
        return Ok(new ApiResponse<List<ComponentInfoResponse>>(0, "操作成功", components));
    }

    [HttpGet("check-update/{key}/{platform}")]
    public async Task<IActionResult> CheckUpdateAsync(string key, PlatformEnum platform)
    {
        var appKey = await _appService.GetAppKeyFromCheckUpdateKeyAsync(key);
        if (appKey.IsEmpty())
        {
            return Ok(new ApiResponse(1, "无可用更新"));
        }
        var appRelease = await _appService.GetAppReleaseInfoAsync(appKey, platform);
        if (appRelease == null)
        {
            return Ok(new ApiResponse(1, "无可用更新"));
        }

        string downloadUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/{appRelease.FilePath}";
        var upgradeInfo = new AppUpgradeInfo
        {
            Name = appRelease.AppKey,
            VersionCode = appRelease.VersionCode,
            Version = appRelease.VersionName,
            MinVersion = appRelease.MinVersionName,
            FileLength = appRelease.FileLength,
            DownloadUrl = downloadUrl,
            CreateTime = appRelease.CreateTime,
            Log = appRelease.UpgradeLog ?? "",
            SignType = appRelease.SignType ?? "",
            SignValue = appRelease.SignValue ?? ""
        };

        return Ok(upgradeInfo);
    }
}