using JiuLing.CommonLibs.Log;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Toolbox.Api.Interface.Services;

namespace Toolbox.Api.Middlewares;
public class AppDownloadMiddleware
{
    private readonly RequestDelegate _next;
    public AppDownloadMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context, IAppService appService)
    {
        if (context.Request.Path.StartsWithSegments("/" + GlobalConfig.AppFolder))
        {
            var path = context.Request.Path.ToUriComponent();
            //这里简单匹配下appKey，也不用校验文件是否真实存在
            (bool success, string appKey) = JiuLing.CommonLibs.Text.RegexUtils.GetOneGroupInFirstMatch(path, @"/apps/(.*?)_");
            if (success)
            {
                await appService.DownloadOnceAsync(appKey);
            }
        }
        await _next.Invoke(context);
    }
}