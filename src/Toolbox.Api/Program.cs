using Microsoft.Extensions.Options;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Toolbox.Api;
using Toolbox.Api.ErrorHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped(serviceProvider => new OpenAIService(new OpenAiOptions()
{
    ApiKey = (serviceProvider.GetService<IOptions<AppSettings>>())?.Value.ChatGPTApiKey ??
             throw new ArgumentNullException()
}));
builder.Services.AddScoped(serviceProvider => new CompletionCreateRequest()
{
    Temperature = 0,
    MaxTokens = 1000
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", x =>
    {
        x.WithOrigins("https://localhost:7161")
            .AllowAnyHeader()
            .AllowAnyMethod();
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
