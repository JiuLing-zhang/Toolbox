using Microsoft.Extensions.Options;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface.Services.OpenAI;
using Toolbox.Api.Models.OpenAI;
using Toolbox.Api.Services.OpenAI;

namespace Toolbox.Api.Factories;

public class OpenAIChatFactory : IOpenAIChatFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    IOptions<AppSettings> _appSettingsOptions;
    public OpenAIChatFactory(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettingsOptions)
    {
        _httpClientFactory = httpClientFactory;
        _appSettingsOptions = appSettingsOptions;
    }
    public IChatService Create(OpenAIModelEnum model, ChatContext context)
    {
        if (model == OpenAIModelEnum.Davinci)
        {
            var config = new DavinciModel(context.Prompt);
            return new DavinciChatService(config, _httpClientFactory, _appSettingsOptions);
        }
        if (model == OpenAIModelEnum.GPT35Turbo)
        {
            if (context.ChatMessages == null)
            {
                throw new ArgumentException("ChatMessages");
            }

            var messages = new List<GPT35TurboMessageModel>();
            foreach (var chatMessage in context.ChatMessages)
            {
                messages.Add(new GPT35TurboMessageModel(chatMessage.Role, chatMessage.Content));
            }
            return new GPT35TurboChatService(new GPT35TurboModel(messages), _httpClientFactory, _appSettingsOptions);
        }
        throw new ArgumentException("不支持的模型");
    }
}