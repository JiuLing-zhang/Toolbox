using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Toolbox.Api.Models;
using Toolbox.Api.Models.Request;

namespace Toolbox.Api.Controllers;
[Route("chatgpt")]
[ApiController]
public class ChatGPTController : ControllerBase
{
    const string OpenAIApi = "https://api.openai.com/v1/chat/completions";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppSettings _appSettings;
    public ChatGPTController(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettingsOptions)
    {
        _httpClientFactory = httpClientFactory;
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
            yield return "11非法请求";
            yield break;
        }
        var requestTime = JiuLing.CommonLibs.Text.TimestampUtils.ConvertToDateTime(request.Timestamp);
        if (DateTime.Now.Subtract(requestTime).TotalSeconds > 120)
        {
            yield return "12请求不可用";
            yield break;
        }

        if (request.Prompt.Length > _appSettings.OpenAI.ContextMaxLength)
        {
            yield return "13内容已超过最大长度限制";
            yield break;
        }

        var messages = new List<OpenAIMessage>();
        messages.Add(new OpenAIMessage("user", request.Prompt));
        if (request.ChatType == Enums.ChatTypeEnum.Coder)
        {
            messages.Add(new OpenAIMessage("system", "我是一个程序员，我专门负责代码相关工作。"));
        }
        var postObj = new OpenAIModel("gpt-3.5-turbo", messages, true);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, OpenAIApi);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appSettings.OpenAI.ChatGPTApiKey);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(postObj), Encoding.UTF8, "application/json");

        var response = await _httpClientFactory.CreateClient("OpenAI").SendAsync(requestMessage);
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var currentLine = await reader.ReadLineAsync();
            if (currentLine.IsEmpty())
            {
                continue;
            }

            if (currentLine.TrimStart().IndexOf("{") == 0)
            {
                var json = $"{currentLine}{await reader.ReadToEndAsync()}";
                var error = JsonSerializer.Deserialize<OpenAIStreamErrorResult>(json);
                var errorString = error?.Error?.Message ?? json;
                yield return $"10{errorString}";
                yield break;
            }

            if (!currentLine.StartsWith("data:"))
            {
                continue;
            }
            currentLine = currentLine.TrimStart(new char[] { 'd', 'a', 't', 'a', ':' }).Trim();
            if (currentLine == "[DONE]")
            {
                yield return "00";
                yield break;
            }
            var streamResult = JsonSerializer.Deserialize<OpenAIStreamResult>(currentLine);
            var data = streamResult?.Choices[0].Delta.Content;
            if (data == null)
            {
                continue;
            }
            await Task.Delay(5);
            yield return $"01{data}";
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

        var messages = new List<OpenAIMessage>();
        messages.Add(new OpenAIMessage("user", request.Prompt));
        if (request.ChatType == Enums.ChatTypeEnum.Coder)
        {
            messages.Add(new OpenAIMessage("system", "我是一个程序员，我专门负责代码相关工作。"));
        }
        var postObj = new OpenAIModel("gpt-3.5-turbo", messages, false);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, OpenAIApi);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appSettings.OpenAI.ChatGPTApiKey);
        requestMessage.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(postObj), Encoding.UTF8, "application/json");
        try
        {
            var response = await _httpClientFactory.CreateClient("OpenAI").SendAsync(requestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();
            var openAIResult = JsonSerializer.Deserialize<OpenAIResult>(responseBody);
            if (openAIResult == null)
            {
                return Ok(new ApiResponse(10, "服务器未返回了空数据"));
            }

            if (openAIResult.Error != null)
            {
                return Ok(new ApiResponse(10, $"出错啦：{openAIResult.Error.Message}"));
            }
            return Ok(new ApiResponse<string>(0, "操作成功", openAIResult.Choices[0].Message.Content));

        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse(10, "服务器连接失败"));
        }
    }
}