using System.Text.Json.Serialization;

namespace Toolbox.Api.Models;

public class OpenAIStreamErrorResult
{
    [JsonPropertyName("error")]
    public OpenAIStreamErrorResultError? Error { get; set; }
}

public class OpenAIStreamErrorResultError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
