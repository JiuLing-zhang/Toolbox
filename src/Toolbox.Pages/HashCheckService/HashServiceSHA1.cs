using Microsoft.JSInterop;

namespace Toolbox.Pages.HashCheckService;
public class HashServiceSHA1 : IHashService
{
    private readonly IJSRuntime _jSRuntime;
    public HashServiceSHA1(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
    }

    public async Task<string> ComputeHashAsync(string text, bool isToUpper)
    {
        var value = await _jSRuntime.InvokeAsync<string>("GetTextSHA1", text);
        return value.ToUpperOrLower(isToUpper);
    }

    public async Task<string> ComputeHashAsync(byte[] buffer, bool isToUpper)
    {
        var value = await _jSRuntime.InvokeAsync<string>("GetFileSHA1", buffer);
        return value.ToUpperOrLower(isToUpper);
    }
}