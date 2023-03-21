using System.Web;
using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Toolbox.Api.Interface;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Response;

namespace Toolbox.Api.Controllers;
[Route("app")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly IAppShowService _appShowService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IAppInfoService _appInfoService;
    public AppController(IAppShowService appShowService, IHostEnvironment hostEnvironment, IAppInfoService appInfoService)
    {
        _appShowService = appShowService;
        _hostEnvironment = hostEnvironment;
        _appInfoService = appInfoService;
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