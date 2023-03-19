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
    public AppController(IAppShowService appShowService, IHostEnvironment hostEnvironment)
    {
        _appShowService = appShowService;
        _hostEnvironment = hostEnvironment;
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
}