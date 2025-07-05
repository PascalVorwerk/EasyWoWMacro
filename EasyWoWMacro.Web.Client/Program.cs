using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EasyWoWMacro.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register services
builder.Services.AddScoped<IConditionalService, ConditionalService>();

await builder.Build().RunAsync();
