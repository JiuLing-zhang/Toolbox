using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Toolbox.Api.Interface.Services.OpenAI;
using Toolbox.Api.Models;
using Toolbox.Api.Models.OpenAI;

namespace Toolbox.Api.Services.OpenAI;
public class GPT35TurboChatService : IChatService
{
    private readonly string _apiAddress = "https://api.openai.com/v1/chat/completions";
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly GPT35TurboModel _model;
    public GPT35TurboChatService(GPT35TurboModel model, IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettingsOptions)
    {
        _model = model;

        _httpClientFactory = httpClientFactory;
        _apiKey = appSettingsOptions.Value.OpenAI.ChatGPTApiKey;
    }

    public async Task<ApiResponse<string>> GetMessageAsync()
    {
        _model.Stream = false;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _apiAddress);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(_model), Encoding.UTF8, "application/json");
        try
        {
            var response = await _httpClientFactory.CreateClient("OpenAI").SendAsync(requestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GPT35TurboResult>(responseBody);
            if (result == null)
            {
                return new ApiResponse<string>(10, "服务器未返回了空数据", null);
            }

            if (result.Error != null)
            {
                return new ApiResponse<string>(10, $"出错啦：{result.Error.Message}", null);
            }
            return new ApiResponse<string>(0, "操作成功", result.Choices?[0].Message.Content);
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>(10, "服务器连接失败", null);
        }
    }

    public async IAsyncEnumerable<string> GetStreamingMessageAsync()
    {
        _model.Stream = true;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _apiAddress);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(_model), Encoding.UTF8, "application/json");

        var response = await _httpClientFactory.CreateClient("OpenAI").SendAsync(requestMessage);
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var currentLine = await reader.ReadLineAsync();
            if (currentLine.IsEmpty())
            {
                yield return $"01 ";
                continue;
            }

            if (currentLine.TrimStart().IndexOf("{") == 0)
            {
                var json = $"{currentLine}{await reader.ReadToEndAsync()}";
                var error = JsonSerializer.Deserialize<OpenAIBaseResult>(json);
                var errorString = error?.Error?.Message ?? json;
                yield return $"10 {errorString}";
                yield break;
            }

            if (!currentLine.StartsWith("data: "))
            {
                continue;
            }
            currentLine = currentLine[6..];
            if (currentLine == "[DONE]")
            {
                yield return "00 ";
                yield break;
            }
            var streamResult = JsonSerializer.Deserialize<GPT35TurboResult>(currentLine);
            var data = streamResult?.Choices?[0].Delta?.Content;
            if (data == null)
            {
                yield return $"01 ";
                continue;
            }
            await Task.Delay(3);
            yield return $"01 {data}";
        }
    }
}