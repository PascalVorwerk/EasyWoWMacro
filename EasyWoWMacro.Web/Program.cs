using EasyWoWMacro.Web.Components;
using EasyWoWMacro.Web.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Register client services for server-side prerendering
builder.Services.AddScoped<IConditionalService, ConditionalService>();

// Add HttpClient for any API calls
builder.Services.AddHttpClient();

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

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(EasyWoWMacro.Web.Client._Imports).Assembly);

// Configure error handling
app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Starting EasyWoWMacro Blazor Web App with WASM interactivity");

app.Run();
