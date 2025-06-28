using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EasyWoWMacro.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add error handling
builder.Services.AddLogging();

var app = builder.Build();

// Configure error handling
app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Starting EasyWoWMacro WebAssembly application");

await app.RunAsync();
