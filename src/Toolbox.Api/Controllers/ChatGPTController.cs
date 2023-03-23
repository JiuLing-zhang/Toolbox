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

    [HttpPost("do-chat")]
    public async Task<IActionResult> DoChat(ChatGPTRequest request)
    {
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
        var postObj = new OpenAIModel("gpt-3.5-turbo", messages);

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