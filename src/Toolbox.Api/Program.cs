﻿using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Toolbox.Api;
using Toolbox.Api.DbContext;
using Toolbox.Api.ErrorHandler;
using Toolbox.Api.Interface;
using Toolbox.Api.Repositories;
using Toolbox.Api.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>
    (options => options.UseNpgsql(builder.Configuration.GetConnectionString("appConnection")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddTransient<IAppPublishService, AppPublishService>();
builder.Services.AddTransient<IAppShowService, AppShowService>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", x =>
    {
        var origins = builder.Configuration["CorsOrigins"];
        if (origins.IsNotEmpty())
        {
            x.WithOrigins(origins.Split(";"))
            .AllowAnyHeader()
            .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("MyPolicy");
app.Run();
