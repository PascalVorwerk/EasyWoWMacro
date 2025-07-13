using EasyWoWMacro.MCP.Server.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EasyWoWMacro.MCP.Server.Testing;

public class MCPTestConsole
{
    private readonly WoWMacroMCPServer _server;
    private readonly ILogger<WoWMacroMCPServer> _logger;

    public MCPTestConsole()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        _logger = loggerFactory.CreateLogger<WoWMacroMCPServer>();
        _server = new WoWMacroMCPServer(_logger);
    }

    public async Task RunInteractiveTestAsync()
    {
        Console.WriteLine("=== WoW Macro MCP Server Test Console ===");
        Console.WriteLine($"Server Health: {(_server.IsHealthy ? "✅ Healthy" : "❌ Unhealthy")}");
        Console.WriteLine();

        await TestServerInfo();
        await TestListTools();
        await TestListResources();
        await TestValidateMacro();
        await InteractiveValidation();
    }

    private async Task TestServerInfo()
    {
        Console.WriteLine("🔍 Testing Server Info...");
        var request = new MCPRequest
        {
            Id = "test-1",
            Method = "initialize"
        };

        var response = await _server.HandleRequestAsync(request);
        Console.WriteLine($"Response: {JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true })}");
        Console.WriteLine();
    }

    private async Task TestListTools()
    {
        Console.WriteLine("🛠️ Testing List Tools...");
        var tools = await _server.ListToolsAsync();
        Console.WriteLine($"Found {tools.Count} tools:");
        foreach (var tool in tools)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }
        Console.WriteLine();
    }

    private async Task TestListResources()
    {
        Console.WriteLine("📚 Testing List Resources...");
        var resources = await _server.ListResourcesAsync();
        Console.WriteLine($"Found {resources.Count} resources:");
        foreach (var resource in resources)
        {
            Console.WriteLine($"  - {resource.Uri}: {resource.Name}");
        }
        Console.WriteLine();
    }

    private async Task TestValidateMacro()
    {
        Console.WriteLine("✅ Testing Macro Validation...");
        
        var testMacros = new[]
        {
            "/cast Fireball",
            "/cast [@target,harm] Fireball; [@target,help] Heal; Fireball",
            "/cast [mod:shift] Ice Barrier; Fireball",
            "",  // Empty macro
            "/cast " + new string('A', 300)  // Too long macro
        };

        foreach (var macroText in testMacros)
        {
            Console.WriteLine($"\nTesting: \"{macroText.Substring(0, Math.Min(50, macroText.Length))}{(macroText.Length > 50 ? "..." : "")}\"");
            
            var arguments = JsonSerializer.SerializeToElement(new { macroText });
            var result = await _server.CallToolAsync("validate_macro", arguments);
            
            var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
                JsonSerializer.Serialize(result.Content));
            
            Console.WriteLine($"  Valid: {validationResult?.IsValid}");
            Console.WriteLine($"  Errors: {validationResult?.Errors.Count ?? 0}");
            Console.WriteLine($"  Character Count: {validationResult?.CharacterCount ?? 0}");
            
            if (validationResult?.Errors.Any() == true)
            {
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"    ❌ {error}");
                }
            }
        }
        Console.WriteLine();
    }

    private async Task InteractiveValidation()
    {
        Console.WriteLine("🎮 Interactive Macro Validation");
        Console.WriteLine("Enter WoW macros to validate (empty line to exit):");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Macro> ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                break;

            try
            {
                var arguments = JsonSerializer.SerializeToElement(new { macroText = input });
                var result = await _server.CallToolAsync("validate_macro", arguments);
                
                var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
                    JsonSerializer.Serialize(result.Content));

                if (validationResult == null)
                {
                    Console.WriteLine("❌ Failed to validate macro");
                    continue;
                }

                Console.WriteLine($"\n📊 Validation Results:");
                Console.WriteLine($"  ✅ Valid: {validationResult.IsValid}");
                Console.WriteLine($"  📏 Length: {validationResult.CharacterCount}/255 characters");
                
                if (validationResult.Errors.Any())
                {
                    Console.WriteLine($"  ❌ Errors ({validationResult.Errors.Count}):");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"    • {error}");
                    }
                }
                
                if (validationResult.Warnings.Any())
                {
                    Console.WriteLine($"  ⚠️ Warnings ({validationResult.Warnings.Count}):");
                    foreach (var warning in validationResult.Warnings)
                    {
                        Console.WriteLine($"    • {warning}");
                    }
                }
                
                if (validationResult.Suggestions.Any())
                {
                    Console.WriteLine($"  💡 Suggestions ({validationResult.Suggestions.Count}):");
                    foreach (var suggestion in validationResult.Suggestions)
                    {
                        Console.WriteLine($"    • {suggestion}");
                    }
                }

                if (!string.IsNullOrEmpty(validationResult.FormattedMacro))
                {
                    Console.WriteLine($"  📝 Formatted Macro:");
                    Console.WriteLine($"    {validationResult.FormattedMacro}");
                }
                
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine();
            }
        }
    }

    public static async Task Main(string[] args)
    {
        var console = new MCPTestConsole();
        await console.RunInteractiveTestAsync();
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}