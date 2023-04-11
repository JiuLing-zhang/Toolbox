using Toolbox.Api.Models;

namespace Toolbox.Api.Interface.Services.OpenAI;
public interface IChatService
{
    IAsyncEnumerable<string> GetStreamingMessageAsync();

    Task<ApiResponse<string>> GetMessageAsync();
}