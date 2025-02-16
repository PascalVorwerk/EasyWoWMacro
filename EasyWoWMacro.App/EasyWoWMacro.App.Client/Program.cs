using EasyWoWMacro.App.Client.Infrastructure;
using EasyWoWMacro.Business;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();
builder.Services.AddClientServices();
builder.Services.AddEasyWoWMacroBusinessServices();

await builder.Build().RunAsync();