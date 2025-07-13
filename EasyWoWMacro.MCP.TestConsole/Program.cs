using EasyWoWMacro.MCP.Server;
using EasyWoWMacro.MCP.Server.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EasyWoWMacro.MCP.TestConsole;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== WoW Macro MCP Server Test Console ===");
        Console.WriteLine();

        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        var logger = loggerFactory.CreateLogger<WoWMacroMCPServer>();
        var server = new WoWMacroMCPServer(logger);

        Console.WriteLine($"Server Health: {(server.IsHealthy ? "✅ Healthy" : "❌ Unhealthy")}");
        Console.WriteLine();

        await TestServerInfo(server);
        await TestListTools(server);
        await TestListResources(server);
        await TestValidateMacro(server);
        await InteractiveValidation(server);
    }

    private static async Task TestServerInfo(WoWMacroMCPServer server)
    {
        Console.WriteLine("🔍 Testing Server Info...");
        var request = new MCPRequest
        {
            Id = "test-1",
            Method = "initialize"
        };

        var response = await server.HandleRequestAsync(request);
        Console.WriteLine($"✅ Response received with ID: {response.Id}");
        Console.WriteLine();
    }

    private static async Task TestListTools(WoWMacroMCPServer server)
    {
        Console.WriteLine("🛠️ Testing List Tools...");
        var tools = await server.ListToolsAsync();
        Console.WriteLine($"Found {tools.Count} tools:");
        foreach (var tool in tools)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }
        Console.WriteLine();
    }

    private static async Task TestListResources(WoWMacroMCPServer server)
    {
        Console.WriteLine("📚 Testing List Resources...");
        var resources = await server.ListResourcesAsync();
        Console.WriteLine($"Found {resources.Count} resources:");
        foreach (var resource in resources)
        {
            Console.WriteLine($"  - {resource.Uri}: {resource.Name}");
        }
        Console.WriteLine();
    }

    private static async Task TestValidateMacro(WoWMacroMCPServer server)
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
            var result = await server.CallToolAsync("validate_macro", arguments);
            
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

    private static async Task InteractiveValidation(WoWMacroMCPServer server)
    {
        Console.WriteLine("🎮 Interactive Macro Validation");
        Console.WriteLine("Enter WoW macros to validate (empty line to exit):");
        Console.WriteLine("Example: /cast [@target,harm] Fireball; [@target,help] Heal; Fireball");
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
                var result = await server.CallToolAsync("validate_macro", arguments);
                
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

        Console.WriteLine("Goodbye! 👋");
    }
}
