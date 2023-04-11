using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Toolbox.Api;
using Toolbox.Api.DbContext;
using Toolbox.Api.ErrorHandler;
using Toolbox.Api.Interface;
using Toolbox.Api.Repositories;
using Toolbox.Api.Services;
using Microsoft.AspNetCore.StaticFiles;
using Toolbox.Api.Interface.Services;
using Toolbox.Api.Interface.Repositories;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Toolbox.Api.Models;
using Toolbox.Api.Middlewares;
using Toolbox.Api.Factories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>
    (options => options.UseNpgsql(builder.Configuration.GetConnectionString("appConnection")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value != null && e.Value.Errors.Count > 0)
            .Select(e => $"{e.Key}:{e.Value?.Errors.First().ErrorMessage}"
          ).ToList();
        return new JsonResult(new ApiResponse(999, $"参数验证失败[{string.Join(",", errors)}]"));
    };
});

builder.Services.AddTransient<IOpenAIChatFactory, OpenAIChatFactory>();
builder.Services.AddTransient<IDatabaseConfigService, DatabaseConfigService>();
builder.Services.AddTransient<IAppService, AppService>();
builder.Services.AddTransient<IAppReleaseRepository, AppReleaseRepository>();
builder.Services.AddTransient<IConfigBaseRepository, ConfigBaseRepository>();
builder.Services.AddTransient<IAppInfoRepository, AppInfoRepository>();
builder.Services.AddTransient<IAppBaseRepository, AppBaseRepository>();
builder.Services.AddTransient<IComponentRepository, ComponentRepository>();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddHttpClient("OpenAI", (sp, client) =>
{
    var appSettings = sp.GetService<IOptions<AppSettings>>()?.Value ?? throw new ArgumentException("配置文件异常");
    if (appSettings.OpenAI.WebProxyAddress.IsNotEmpty())
    {
        var handler = new HttpClientHandler()
        {
            Proxy = new WebProxy(appSettings.OpenAI.WebProxyAddress)
        };
        client = new HttpClient(handler);
    }
});

builder.Services.AddCors(setup =>
{
    setup.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100000000;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<AppDownloadMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

var fectProvider = new FileExtensionContentTypeProvider();
fectProvider.Mappings.Add(".apk", "application/vnd.android.package-archive");
fectProvider.Mappings.Add(".msix", "application/octet-stream");

var appFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", GlobalConfig.AppFolder);
if (!Directory.Exists(appFolderPath))
{
    Directory.CreateDirectory(appFolderPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(appFolderPath),
    RequestPath = "/" + GlobalConfig.AppFolder,
    ContentTypeProvider = fectProvider
});

app.MapControllers();
app.UseCors();
app.Run();
