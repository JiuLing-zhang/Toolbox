using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using JiuLing.CommonLibs.Log;
using Toolbox.Api.Models;
using ILogger = JiuLing.CommonLibs.Log.ILogger;

namespace Toolbox.Api.ErrorHandler;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _logger = LogManager.GetLogger();
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            try
            {
                string message = "";
                message = $"{message}---------------{Environment.NewLine}";
                message = $"{message}系统内部异常。{Environment.NewLine}";
                message = $"{message}{ex.Message}。{Environment.NewLine}";
                message = $"{message}{ex.StackTrace}。{Environment.NewLine}";
                var innerException = ex.InnerException;
                if (innerException != null)
                {
                    message = $"{message}↓↓↓↓↓InnerException↓↓↓↓↓{Environment.NewLine}";
                    message = $"{message}{innerException.Message}。{Environment.NewLine}";
                    message = $"{message}{innerException.StackTrace}。{Environment.NewLine}";
                }
                _logger.Write(message);
            }
            catch (Exception)
            {

            }

            var response = context.Response;
            response.ContentType = "application/json";

            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var result = JsonSerializer.Serialize(new ApiResponse(-1, "系统内部异常"), jsonOptions);
            await response.WriteAsync(result);
        }
    }
}