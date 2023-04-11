using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;

public class GPT35TurboResult : OpenAIBaseResult<List<GPT35TurboChoice>>
{

}

public class GPT35TurboChoice
{
    [JsonPropertyName("message")]
    public GPT35TurboChoiceMessage? Message { get; set; } = null!;

    [JsonPropertyName("delta")]
    public GPT35TurboChoiceMessage? Delta { get; set; } = null!;
}

public class GPT35TurboChoiceMessage
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}
