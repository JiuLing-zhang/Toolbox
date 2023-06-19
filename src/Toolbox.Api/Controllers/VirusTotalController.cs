using Microsoft.AspNetCore.Mvc;
using Toolbox.Api.Interface.Services;
using Toolbox.Api.Models;
using Toolbox.Api.Models.VirusTotal;

namespace Toolbox.Api.Controllers;

[Route("virustotal")]
[ApiController]
public class VirusTotalController : ControllerBase
{
    private readonly IVirusTotalService _virusTotalService;
    public VirusTotalController(IVirusTotalService virusTotalService)
    {
        _virusTotalService = virusTotalService;
    }

    [HttpGet("file/{sha1}")]
    public async Task<IActionResult> GetAppsAsync(string sha1)
    {
        var result = await _virusTotalService.CheckByFileHashAsync(sha1);
        if (!result.Succeed)
        {
            return Ok(new ApiResponse(1, result.Message));
        }
        return Ok(new ApiResponse<AnalysisRelustResponse>(0, "", result.Data));
    }
}