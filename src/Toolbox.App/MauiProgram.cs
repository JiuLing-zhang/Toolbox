using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using Toolbox.Pages;
using Toolbox.Pages.HashCheckService;

namespace Toolbox.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonStream(stream).Build());

            builder.Services.AddHttpClient("WebAPI", (sp, client) =>
            {
                client.BaseAddress = new Uri(sp.GetService<IConfiguration>()?.GetValue<string>("WebAPIHost") ?? throw new ArgumentException());
            });
            builder.Services.AddHttpClient("ChatAPI", (sp, client) =>
            {
                client.BaseAddress = new Uri(sp.GetService<IConfiguration>()?.GetValue<string>("ChatAPIHost") ?? throw new ArgumentException());
            });
            builder.Services.AddHttpClient("VirusTotal", (sp, client) =>
            {
                client.BaseAddress = new Uri(sp.GetService<IConfiguration>()?.GetValue<string>("VirusTotalAPIHost") ?? throw new ArgumentException());
            });
            builder.Services.AddSingleton<HashServiceFactory>();
            builder.Services.AddMudServices();
            builder.Services.AddMudMarkdownServices();

            PlatformSettings.IsApp = true;
            return builder.Build();
        }
    }
}