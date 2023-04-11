using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;

public class OpenAIBaseResult
{
    [JsonPropertyName("error")]
    public OpenAIBaseError? Error { get; set; }
}

public class OpenAIBaseResult<TChoice> : OpenAIBaseResult
{
    [JsonPropertyName("choices")]
    public TChoice? Choices { get; set; }
}

public class OpenAIBaseError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}
