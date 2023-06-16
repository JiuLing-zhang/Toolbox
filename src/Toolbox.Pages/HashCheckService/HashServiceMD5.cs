using Microsoft.JSInterop;

namespace Toolbox.Pages.HashCheckService;
public class HashServiceMD5 : IHashService
{
    private readonly IJSRuntime _jSRuntime;
    public HashServiceMD5(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
    }
    public async Task<string> ComputeHashAsync(string text, bool isToUpper)
    {
        var value = await _jSRuntime.InvokeAsync<string>("GetTextMD5", text);
        return value.ToUpperOrLower(isToUpper);
    }

    public async Task<string> ComputeHashAsync(byte[] buffer, bool isToUpper)
    {
        var value = await _jSRuntime.InvokeAsync<string>("GetFileMD5", buffer);
        return value.ToUpperOrLower(isToUpper);
    }
}

//TODO 移入公共类库
public static class Temp
{
    public static string ToUpperOrLower(this string value, bool isToUpper)
    {
        if (isToUpper)
        {
            return value.ToUpper();
        }
        else
        {
            return value.ToLower();
        }
    }
}