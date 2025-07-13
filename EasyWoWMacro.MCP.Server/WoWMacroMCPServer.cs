using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.MCP.Server.Interfaces;
using EasyWoWMacro.MCP.Server.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EasyWoWMacro.MCP.Server;

public class WoWMacroMCPServer : IMCPServer
{
    private readonly ILogger<WoWMacroMCPServer> _logger;
    private readonly MacroParser _macroParser;
    private bool _isHealthy = true;

    public WoWMacroMCPServer(ILogger<WoWMacroMCPServer> logger)
    {
        _logger = logger;
        _macroParser = new MacroParser();
    }

    public bool IsHealthy => _isHealthy;

    public async Task<MCPResponse> HandleRequestAsync(MCPRequest request)
    {
        try
        {
            _logger.LogDebug("Handling MCP request: {Method}", request.Method);

            var result = request.Method switch
            {
                "initialize" => await GetServerInfoAsync(),
                "tools/list" => await ListToolsAsync(),
                "resources/list" => await ListResourcesAsync(),
                "tools/call" => await HandleToolCallAsync(request.Params),
                "resources/read" => await HandleResourceReadAsync(request.Params),
                _ => throw new InvalidOperationException($"Unknown method: {request.Method}")
            };

            return new MCPResponse
            {
                Id = request.Id,
                Result = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MCP request: {Method}", request.Method);
            
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError
                {
                    Code = -32603,
                    Message = "Internal error",
                    Data = ex.Message
                }
            };
        }
    }

    public Task<ServerInfo> GetServerInfoAsync()
    {
        var serverInfo = new ServerInfo
        {
            Name = "WoW Macro MCP Server",
            Version = "1.0.0",
            ProtocolVersion = "2025-06-18"
        };

        _logger.LogInformation("Server info requested: {Name} v{Version}", serverInfo.Name, serverInfo.Version);
        return Task.FromResult(serverInfo);
    }

    public Task<List<MCPTool>> ListToolsAsync()
    {
        var tools = new List<MCPTool>
        {
            new()
            {
                Name = "validate_macro",
                Description = "Validates a complete WoW macro for syntax and semantic correctness",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        macroText = new
                        {
                            type = "string",
                            description = "The complete macro text to validate"
                        }
                    },
                    required = new[] { "macroText" }
                }
            }
        };

        _logger.LogDebug("Listed {Count} tools", tools.Count);
        return Task.FromResult(tools);
    }

    public Task<List<MCPResource>> ListResourcesAsync()
    {
        var resources = new List<MCPResource>
        {
            new()
            {
                Uri = "wow://commands",
                Name = "WoW Commands",
                Description = "Complete list of valid World of Warcraft slash commands",
                MimeType = "application/json"
            },
            new()
            {
                Uri = "wow://conditionals",
                Name = "WoW Conditionals",
                Description = "Complete list of valid World of Warcraft macro conditionals",
                MimeType = "application/json"
            }
        };

        _logger.LogDebug("Listed {Count} resources", resources.Count);
        return Task.FromResult(resources);
    }

    public async Task<ToolResult> CallToolAsync(string toolName, object? arguments)
    {
        try
        {
            var result = toolName switch
            {
                "validate_macro" => await ValidateMacroAsync(arguments),
                _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool: {ToolName}", toolName);
            
            return new ToolResult
            {
                Content = new { error = ex.Message },
                IsError = true
            };
        }
    }

    public async Task<ResourceResult> ReadResourceAsync(string uri)
    {
        try
        {
            var content = uri switch
            {
                "wow://commands" => WoWMacroConstants.ValidSlashCommands,
                "wow://conditionals" => ConditionalValidator.GetValidConditionalKeys().ToArray(),
                _ => throw new InvalidOperationException($"Unknown resource: {uri}")
            };

            return new ResourceResult
            {
                Contents = content,
                MimeType = "application/json"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading resource: {Uri}", uri);
            throw;
        }
    }

    private async Task<object> HandleToolCallAsync(object? parameters)
    {
        if (parameters is JsonElement jsonParams)
        {
            var toolName = jsonParams.GetProperty("name").GetString();
            var arguments = jsonParams.TryGetProperty("arguments", out var args) ? args : (object?)null;
            
            return await CallToolAsync(toolName ?? string.Empty, arguments);
        }

        throw new InvalidOperationException("Invalid tool call parameters");
    }

    private async Task<object> HandleResourceReadAsync(object? parameters)
    {
        if (parameters is JsonElement jsonParams)
        {
            var uri = jsonParams.GetProperty("uri").GetString();
            return await ReadResourceAsync(uri ?? string.Empty);
        }

        throw new InvalidOperationException("Invalid resource read parameters");
    }

    private Task<ToolResult> ValidateMacroAsync(object? arguments)
    {
        try
        {
            string macroText = string.Empty;

            if (arguments is JsonElement jsonArgs)
            {
                macroText = jsonArgs.GetProperty("macroText").GetString() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(macroText))
            {
                return Task.FromResult(new ToolResult
                {
                    Content = new MacroValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { "Macro text is required" }
                    },
                    IsError = false
                });
            }

            var macro = _macroParser.Parse(macroText);
            var errors = new List<string>();
            var warnings = new List<string>();
            var suggestions = new List<string>();

            var bracketErrors = _macroParser.ValidateBrackets(macroText);
            errors.AddRange(bracketErrors);

            var formattedMacro = macro.GetFormattedMacro();
            var characterCount = formattedMacro.Length;

            if (characterCount > 255)
            {
                errors.Add($"Macro exceeds 255 character limit ({characterCount} characters)");
            }
            else if (characterCount > 230)
            {
                warnings.Add($"Macro is {characterCount} characters, close to 255 limit");
            }

            if (characterCount > 200)
            {
                suggestions.Add("Consider using shorter command abbreviations to save characters");
            }

            var validationResult = new MacroValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings,
                Suggestions = suggestions,
                FormattedMacro = formattedMacro,
                CharacterCount = characterCount
            };

            _logger.LogDebug("Validated macro: {IsValid}, {ErrorCount} errors, {WarningCount} warnings", 
                validationResult.IsValid, errors.Count, warnings.Count);

            return Task.FromResult(new ToolResult
            {
                Content = validationResult,
                IsError = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating macro");
            
            return Task.FromResult(new ToolResult
            {
                Content = new MacroValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Validation error: {ex.Message}" }
                },
                IsError = true
            });
        }
    }
}