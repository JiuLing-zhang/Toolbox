using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;
public class GPT35TurboModel : OpenAIBaseModel
{
    [JsonPropertyName("messages")]
    public List<GPT35TurboMessageModel> Messages { get; set; }
    public GPT35TurboModel(List<GPT35TurboMessageModel> messages) : base("gpt-3.5-turbo", 1500, true)
    {
        Messages = messages;
    }
}
public class GPT35TurboMessageModel
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    public GPT35TurboMessageModel(string role, string content)
    {
        Role = role;
        Content = content;
    }
}