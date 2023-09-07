global using EGXMonitoring.Shared;
global using EGXMonitoring.Shared.DTOS;
global using System.Net.Http.Json;
global using EGXMonitoring.Client.Services.AuthService;
global using EGXMonitoring.Client.Services.WidgetService;
global using EGXMonitoring.Client.Services.UsersService;
global using Microsoft.AspNetCore.Components.Authorization;

using Blazored.LocalStorage;
using EGXMonitoring.Client;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTelerikBlazor();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IWidgetService, WidgetService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
await builder.Build().RunAsync();
