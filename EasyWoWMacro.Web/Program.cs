using EasyWoWMacro.Web.Components;
using EasyWoWMacro.Web.Client.Services;
using EasyWoWMacro.Web.Services;
using EasyWoWMacro.Web.Services.Interfaces;
using EasyWoWMacro.MCP.Server;
using EasyWoWMacro.MCP.Server.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add API controllers
builder.Services.AddControllers();

// Register client services for server-side prerendering
builder.Services.AddScoped<IConditionalService, ConditionalService>();

// Add HttpClient for LLM service
builder.Services.AddHttpClient<ILLMService, OpenRouterLLMService>();

// Configure OpenRouter options
builder.Services.Configure<OpenRouterOptions>(builder.Configuration.GetSection(OpenRouterOptions.SectionName));

// Register MCP services
builder.Services.AddSingleton<IMCPServer, WoWMacroMCPServer>();
builder.Services.AddScoped<IMCPClientService, LocalMCPClientService>();

// Register macro generation service
builder.Services.AddScoped<IMacroGenerationService, MacroGenerationService>();

// Add error handling
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

// Map API controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(EasyWoWMacro.Web.Client._Imports).Assembly);

// Configure error handling
app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Starting EasyWoWMacro Blazor Web App with WASM interactivity");

app.Run();
