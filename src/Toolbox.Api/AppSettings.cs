namespace Toolbox.Api;
public class AppSettings
{
    public string[] CorsOrigins { get; set; } = null!;
    public OpenAIAppSettings OpenAI { get; set; } = null!;
}

public class OpenAIAppSettings
{
    public string WebProxyAddress { get; set; } = null!;
    public string ChatGPTApiKey { get; set; } = null!;
    public int ContextMaxLength { get; set; }
}