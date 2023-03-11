using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbox.Web;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("WebAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebAPIHost") ?? throw new ArgumentException());
});

builder.Services.AddMudServices();

await builder.Build().RunAsync();
