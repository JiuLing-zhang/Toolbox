using System.Text.Json.Serialization;

namespace Toolbox.Api.Models;

public class OpenAIStreamResult
{
    [JsonPropertyName("choices")]
    public OpenAIStreamChoiceResult[] Choices { get; set; } = null!;
}

public class OpenAIStreamChoiceResult
{
    [JsonPropertyName("delta")]
    public OpenAIStreamChoiceDeltaResult Delta { get; set; } = null!;
}

public class OpenAIStreamChoiceDeltaResult
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}
