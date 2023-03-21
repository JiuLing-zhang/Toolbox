using System.Text.Json.Serialization;

namespace Toolbox.Api.Models;

public class OpenAIResult
{
    [JsonPropertyName("error")]
    public OpenAIError? Error { get; set; }  
    
    [JsonPropertyName("choices")]
    public List<OpenAIChoice> Choices { get; set; } = null!;
}

public class OpenAIError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}

public class OpenAIChoice
{
    [JsonPropertyName("message")]
    public OpenAIChoiceMessage Message { get; set; } = null!;
}

public class OpenAIChoiceMessage
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}
