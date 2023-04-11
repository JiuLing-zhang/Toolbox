using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;
public class OpenAIBaseModel
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    public OpenAIBaseModel(string model, int maxTokens, bool stream)
    {
        Model = model;
        MaxTokens = maxTokens;
        Stream = stream;
    }
}