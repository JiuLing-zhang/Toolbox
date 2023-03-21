using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbox.Web;
using MudBlazor.Services;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build());
builder.Services.AddHttpClient("WebAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebAPIHost") ?? throw new ArgumentException());
});

builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();
await builder.Build().RunAsync();
