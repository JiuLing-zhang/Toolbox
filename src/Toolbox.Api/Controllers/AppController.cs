using System.IO;
using System.Web;
using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IAppShowService _appShowService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IAppInfoService _appInfoService;
    private readonly IAppService _appService;
    private readonly IDatabaseConfigService _databaseConfigService;
    public AppController(IAppShowService appShowService, IHostEnvironment hostEnvironment, IAppInfoService appInfoService, IAppService appService, IDatabaseConfigService databaseConfigService)
    {
        _appShowService = appShowService;
        _hostEnvironment = hostEnvironment;
        _appInfoService = appInfoService;
        _appService = appService;
        _databaseConfigService = databaseConfigService;
    }

    [HttpPost("app-publish")]
    public async Task<IActionResult> Publish([FromForm] AppPublishRequest request)
    {
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
        var fileName = $"{request.AppKey}-{request.Platform}-{request.VersionName}-{random}".ToLower();

        var fileExtension = Path.GetExtension(Path.GetFileName(request.File.FileName));
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
            await request.File.CopyToAsync(stream);
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
            (int)request.File.Length,
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
        var apps = await _appShowService.GetAppsAsync();
        return Ok(new ApiResponse<List<AppInfoResponse>>(0, "操作成功", apps));
    }

    [HttpGet("component-list")]
    public async Task<IActionResult> GetComponentsAsync()
    {
        var components = await _appShowService.GetComponentsAsync();
        return Ok(new ApiResponse<List<ComponentInfoResponse>>(0, "操作成功", components));
    }


    [HttpGet("download/{id}")]
    public async Task<IActionResult> Download(string id)
    {
        var (filePath, contentType) = await _appInfoService.GetDownloadInfoAsync(id);
        if (filePath.IsEmpty())
        {
            return NotFound("文件信息错误");
        }
        var tempPath = HttpUtility.UrlDecode(filePath).Replace("/", "\\");
        string fileRealPath = Path.Combine(_hostEnvironment.ContentRootPath, tempPath);
        if (!System.IO.File.Exists(fileRealPath))
        {
            return NotFound("文件不存在");
        }
        var stream = System.IO.File.OpenRead(fileRealPath);
        return File(stream, contentType, Path.GetFileName(tempPath));
    }
}