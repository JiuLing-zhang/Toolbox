using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;
public class DavinciModel : OpenAIBaseModel
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
    public DavinciModel(string prompt) : base("text-davinci-003", 1500, true)
    {
        Prompt = prompt;
    }
}