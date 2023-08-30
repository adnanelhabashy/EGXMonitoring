global using EGXMonitoring.Shared;
global using EGXMonitoring.Shared.DTOS;
using EGXMonitoring.Client;
using EGXMonitoring.Client.Services.WidgetService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IWidgetService, WidgetService>();
await builder.Build().RunAsync();
