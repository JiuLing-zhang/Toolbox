using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.OpenAI;
public class DavinciResult : OpenAIBaseResult<List<DavinciChoice>>
{
}

public class DavinciChoice
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;

}