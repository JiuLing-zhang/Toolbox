using Toolbox.Api.Enums;
using Toolbox.Api.Interface.Services.OpenAI;
using Toolbox.Api.Models.OpenAI;

namespace Toolbox.Api.Factories;
public interface IOpenAIChatFactory
{
    IChatService Create(OpenAIModelEnum model, ChatContext context);
}