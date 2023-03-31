using System.Text.Json.Serialization;

namespace Toolbox.Api.Models;
internal class OpenAIModel
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("messages")]
    public List<OpenAIMessage> Messages { get; set; }
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    public OpenAIModel(string model, List<OpenAIMessage> messages, bool stream)
    {
        Model = model;
        Messages = messages;
        Stream = stream;
    }
}

internal class OpenAIMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    public OpenAIMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
}