using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Toolbox.Pages.Enums;

namespace Toolbox.Pages.HashCheckService;
public class HashServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    public HashServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IHashService Create(HashTypeEnum hashType)
    {
        var jsRuntime = _serviceProvider.GetRequiredService<IJSRuntime>();
        return hashType switch
        {
            HashTypeEnum.MD5 => new HashServiceMD5(jsRuntime),
            HashTypeEnum.SHA1 => new HashServiceSHA1(jsRuntime),
            HashTypeEnum.SHA256 => new HashServiceSHA256(jsRuntime),
            _ => throw new Exception("无效的哈希类型"),
        };
    }
}