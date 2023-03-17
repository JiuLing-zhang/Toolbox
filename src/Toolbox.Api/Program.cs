using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.Extensions.Options;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System.Net;
using Toolbox.Api;
using Toolbox.Api.ErrorHandler;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddHttpClient("OpenAI", (sp, client) =>
{
    var address = sp.GetService<IOptions<AppSettings>>()?.Value.OpenAI.WebProxyAddress;
    if (address.IsNotEmpty())
    {
        var handler = new HttpClientHandler()
        {
            Proxy = new WebProxy(address)
        };
        client = new HttpClient(handler);
    }
});

builder.Services.AddScoped<OpenAIService>(sp =>
{
    return new OpenAIService(new OpenAiOptions()
    {
        ApiKey = sp.GetService<IOptions<AppSettings>>()?.Value.OpenAI.ChatGPTApiKey ??
                  throw new ArgumentNullException()

    }, sp.GetService<IHttpClientFactory>()?.CreateClient("OpenAI"));
});
builder.Services.AddScoped(serviceProvider => new CompletionCreateRequest()
{
    Temperature = 0,
    MaxTokens = 1000
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
