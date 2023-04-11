using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Toolbox.Api.Factories;
using Toolbox.Api.Models;
using Toolbox.Api.Models.OpenAI;
using Toolbox.Api.Models.Request;

namespace Toolbox.Api.Controllers;
[Route("chatgpt")]
[ApiController]
public class ChatGPTController : ControllerBase
{
    private readonly IOpenAIChatFactory _openAIChatFactory;
    private readonly AppSettings _appSettings;
    public ChatGPTController(IOpenAIChatFactory openAIChatFactory, IOptions<AppSettings> appSettingsOptions)
    {
        _openAIChatFactory = openAIChatFactory;
        _appSettings = appSettingsOptions.Value;
    }

    [HttpGet("test")]
    public async IAsyncEnumerable<string> Test()
    {
        for (int i = 0; i < 1000; i++)
        {
            await Task.Delay(5);
            yield return $"{i}";
        }
    }

    [HttpPost("test")]
    public async IAsyncEnumerable<string> Test2()
    {
        for (int i = 0; i < 1000; i++)
        {
            await Task.Delay(5);
            yield return $"{i}";
        }
    }

    [HttpPost("do-chat-streaming")]
    public async IAsyncEnumerable<string> GetStreaming(ChatGPTRequest request)
    {
        var sign = JiuLing.CommonLibs.Security.SHA1Utils.GetStringValueToLower($"{request.Timestamp}{request.Timestamp}{request.Timestamp}");
        if (sign != request.Sign)
        {
            yield return "11 非法请求";
            yield break;
        }
        var requestTime = JiuLing.CommonLibs.Text.TimestampUtils.ConvertToDateTime(request.Timestamp);
        if (DateTime.Now.Subtract(requestTime).TotalSeconds > 120)
        {
            yield return "12 请求不可用";
            yield break;
        }

        if (request.Prompt.Length > _appSettings.OpenAI.ContextMaxLength)
        {
            yield return "13 内容已超过最大长度限制";
            yield break;
        }

        var chatMessages = new List<ChatMessage>();
        chatMessages.Add(new ChatMessage("user", request.Prompt));
        if (request.ChatType == Enums.ChatTypeEnum.Coder)
        {
            chatMessages.Add(new ChatMessage("system", "我是一个程序员，我专门负责代码相关工作。"));
        }
        var chatContext = new ChatContext(request.Prompt, chatMessages);

        await foreach (var item in _openAIChatFactory.Create(request.Model, chatContext).GetStreamingMessageAsync())
        {
            yield return item;
        }
    }

    [HttpPost("do-chat")]
    public async Task<IActionResult> DoChat(ChatGPTRequest request)
    {
        var sign = JiuLing.CommonLibs.Security.SHA1Utils.GetStringValueToLower($"{request.Timestamp}{request.Timestamp}{request.Timestamp}");
        if (sign != request.Sign)
        {
            return Ok(new ApiResponse(12, "非法请求"));
        }
        var requestTime = JiuLing.CommonLibs.Text.TimestampUtils.ConvertToDateTime(request.Timestamp);
        if (DateTime.Now.Subtract(requestTime).TotalSeconds > 120)
        {
            return Ok(new ApiResponse(13, "请求不可用"));
        }

        if (request.Prompt.Length > _appSettings.OpenAI.ContextMaxLength)
        {
            return Ok(new ApiResponse(1, "内容已超过最大长度限制"));
        }

        var chatMessages = new List<ChatMessage>();
        chatMessages.Add(new ChatMessage("user", request.Prompt));
        if (request.ChatType == Enums.ChatTypeEnum.Coder)
        {
            chatMessages.Add(new ChatMessage("system", "我是一个程序员，我专门负责代码相关工作。"));
        }
        var chatContext = new ChatContext(request.Prompt, chatMessages);
        return Ok(await _openAIChatFactory.Create(request.Model, chatContext).GetMessageAsync());
    }
}