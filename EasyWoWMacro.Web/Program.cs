using EasyWoWMacro.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for any API calls
builder.Services.AddHttpClient();

// Add error handling
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Configure error handling
app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Starting EasyWoWMacro Blazor Server application");

app.Run();
