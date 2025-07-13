using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EasyWoWMacro.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register services
builder.Services.AddScoped<IConditionalService, ConditionalService>();

await builder.Build().RunAsync();
