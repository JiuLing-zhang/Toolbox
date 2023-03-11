using Microsoft.AspNetCore.Mvc;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Toolbox.Api.Models;

namespace Toolbox.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ChatGPTController : ControllerBase
{
    private readonly OpenAIService _service;
    private readonly CompletionCreateRequest _createRequest;
    public ChatGPTController(OpenAIService service, CompletionCreateRequest createRequest)
    {
        _service = service;
        _createRequest = createRequest;
    }

    [HttpPost]
    public async Task<IActionResult> DoChat(string prompt)
    {
        _createRequest.Prompt = prompt;
        var res = await _service.Completions.CreateCompletion(_createRequest, OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);

        if (!res.Successful)
        {
            return Ok(new ApiResponse(1, res.Error?.Message ?? "对方似乎没搞懂你想干啥...."));
        }
        return Ok(new ApiResponse<string>(0, "操作成功", res.Choices.First().Text.Trim('\n')));

    }
}