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

    [HttpPost("do-chat-streaming")]
    public async Task GetStreaming(ChatGPTRequest request)
    {
        var outputStream = this.Response.Body;
        try
        {
            var sign = JiuLing.CommonLibs.Security.SHA1Utils.GetStringValueToLower($"{request.Timestamp}{request.Timestamp}{request.Timestamp}");
            if (sign != request.Sign)
            {
                await outputStream.WriteAsync(Encoding.UTF8.GetBytes("error:12:非法请求"));
                return;
            }

            if (request.Prompt.Length > _appSettings.OpenAI.ContextMaxLength)
            {
                await outputStream.WriteAsync(Encoding.UTF8.GetBytes("error:11:内容已超过最大长度限制"));
                return;
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
                    await outputStream.WriteAsync(Encoding.UTF8.GetBytes($"error:10:{errorString}"));
                    await outputStream.FlushAsync();
                    return;
                }

                if (!currentLine.StartsWith("data:"))
                {
                    continue;
                }
                currentLine = currentLine.TrimStart(new char[] { 'd', 'a', 't', 'a', ':' }).Trim();
                if (currentLine == "[DONE]")
                {
                    await outputStream.WriteAsync(Encoding.UTF8.GetBytes($"data:[DONE]{Environment.NewLine}"));
                    await outputStream.FlushAsync();
                    return;
                }
                var streamResult = JsonSerializer.Deserialize<OpenAIStreamResult>(currentLine);
                var data = streamResult?.Choices[0].Delta.Content;
                if (data == null)
                {
                    continue;
                }
                string result = $"data:{data}{Environment.NewLine}";
                await outputStream.WriteAsync(Encoding.UTF8.GetBytes(result));
                await outputStream.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            await outputStream.WriteAsync(Encoding.UTF8.GetBytes("error:10:服务器连接失败"));
            await outputStream.FlushAsync();
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