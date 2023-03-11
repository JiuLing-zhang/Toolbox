using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Request;

namespace Toolbox.Api.Controllers;
[Route("chatgpt")]
[ApiController]
public class ChatGPTController : ControllerBase
{
    private readonly OpenAIService _service;
    private readonly CompletionCreateRequest _createRequest;
    private readonly AppSettings _appSettings;
    public ChatGPTController(OpenAIService service, CompletionCreateRequest createRequest, IOptions<AppSettings> appSettingsOptions)
    {
        _service = service;
        _createRequest = createRequest;
        _appSettings = appSettingsOptions.Value;
    }

    [HttpPost("do-chat")]
    public async Task<IActionResult> DoChat(ChatGPTRequest request)
    {
        if (request.Prompt.Length > _appSettings.ContextMaxLength)
        {
            return Ok(new ApiResponse(1, "语境有点长了，请刷新页面后重新玩耍~~~~"));
        }
        _createRequest.Prompt = request.Prompt;
        var res = await _service.Completions.CreateCompletion(_createRequest, OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);

        if (!res.Successful)
        {
            return Ok(new ApiResponse(2, res.Error?.Message ?? "对方似乎没搞懂你想干啥...."));
        }
        return Ok(new ApiResponse<string>(0, "操作成功", res.Choices.First().Text.Trim('\n')));

    }
}