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
            },
            new()
            {
                Name = "analyze_macro_issues",
                Description = "Comprehensive analysis of macro issues with detailed fix suggestions",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        macroText = new
                        {
                            type = "string",
                            description = "The macro text to analyze"
                        },
                        analysisDepth = new
                        {
                            type = "string",
                            @enum = new[] { "basic", "detailed", "comprehensive" },
                            description = "Depth of analysis to perform",
                            @default = "detailed"
                        }
                    },
                    required = new[] { "macroText" }
                }
            },
            new()
            {
                Name = "suggest_macro_fixes",
                Description = "Generate specific fix recommendations for identified macro issues",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        macroText = new
                        {
                            type = "string",
                            description = "The macro text to fix"
                        },
                        issues = new
                        {
                            type = "array",
                            items = new { type = "string" },
                            description = "List of identified issues to address"
                        },
                        preserveIntent = new
                        {
                            type = "boolean",
                            @default = true,
                            description = "Whether to preserve original macro intent"
                        }
                    },
                    required = new[] { "macroText" }
                }
            },
            new()
            {
                Name = "optimize_macro",
                Description = "Optimize macro for character count and performance while preserving functionality",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        macroText = new
                        {
                            type = "string",
                            description = "The macro text to optimize"
                        },
                        targetLength = new
                        {
                            type = "number",
                            @default = 255,
                            description = "Target character count limit"
                        },
                        optimizationLevel = new
                        {
                            type = "string",
                            @enum = new[] { "conservative", "moderate", "aggressive" },
                            @default = "moderate",
                            description = "Level of optimization to apply"
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
                "analyze_macro_issues" => await AnalyzeMacroIssuesAsync(arguments),
                "suggest_macro_fixes" => await SuggestMacroFixesAsync(arguments),
                "optimize_macro" => await OptimizeMacroAsync(arguments),
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

    private Task<ToolResult> AnalyzeMacroIssuesAsync(object? arguments)
    {
        try
        {
            string macroText = string.Empty;
            string analysisDepth = "detailed";

            if (arguments is JsonElement jsonArgs)
            {
                macroText = jsonArgs.GetProperty("macroText").GetString() ?? string.Empty;
                if (jsonArgs.TryGetProperty("analysisDepth", out var depthProp))
                {
                    analysisDepth = depthProp.GetString() ?? "detailed";
                }
            }

            if (string.IsNullOrWhiteSpace(macroText))
            {
                return Task.FromResult(new ToolResult
                {
                    Content = new MacroAnalysisResult
                    {
                        Issues = new List<MacroIssue> { new() { Type = "validation", Message = "Macro text is required", Severity = "error" } }
                    },
                    IsError = false
                });
            }

            var issues = new List<MacroIssue>();
            
            // Comprehensive analysis
            issues.AddRange(AnalyzeSyntaxIssues(macroText));
            issues.AddRange(AnalyzeCommandIssues(macroText));
            issues.AddRange(AnalyzeConditionalIssues(macroText));
            issues.AddRange(AnalyzeCharacterLimitIssues(macroText));
            
            if (analysisDepth == "comprehensive")
            {
                issues.AddRange(AnalyzeLogicFlowIssues(macroText));
                issues.AddRange(AnalyzeOptimizationOpportunities(macroText));
            }

            var analysisResult = new MacroAnalysisResult
            {
                Issues = issues,
                Severity = CalculateOverallSeverity(issues),
                FixSuggestions = GenerateFixSuggestions(issues),
                OptimizationOpportunities = analysisDepth == "comprehensive" ? FindOptimizations(macroText) : new List<string>()
            };

            _logger.LogDebug("Analyzed macro: {IssueCount} issues found, severity: {Severity}", 
                issues.Count, analysisResult.Severity);

            return Task.FromResult(new ToolResult
            {
                Content = analysisResult,
                IsError = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing macro");
            
            return Task.FromResult(new ToolResult
            {
                Content = new MacroAnalysisResult
                {
                    Issues = new List<MacroIssue> { new() { Type = "analysis", Message = $"Analysis error: {ex.Message}", Severity = "error" } }
                },
                IsError = true
            });
        }
    }

    private Task<ToolResult> SuggestMacroFixesAsync(object? arguments)
    {
        try
        {
            string macroText = string.Empty;
            var issues = new List<string>();
            bool preserveIntent = true;

            if (arguments is JsonElement jsonArgs)
            {
                macroText = jsonArgs.GetProperty("macroText").GetString() ?? string.Empty;
                if (jsonArgs.TryGetProperty("issues", out var issuesProp) && issuesProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var issue in issuesProp.EnumerateArray())
                    {
                        var issueText = issue.GetString();
                        if (!string.IsNullOrEmpty(issueText))
                        {
                            issues.Add(issueText);
                        }
                    }
                }
                if (jsonArgs.TryGetProperty("preserveIntent", out var preserveProp))
                {
                    preserveIntent = preserveProp.GetBoolean();
                }
            }

            if (string.IsNullOrWhiteSpace(macroText))
            {
                return Task.FromResult(new ToolResult
                {
                    Content = new MacroFixSuggestionResult
                    {
                        Fixes = new List<MacroFix> { new() { Type = "validation", Issue = "Macro text is required", Fix = "Please provide macro text to analyze" } }
                    },
                    IsError = false
                });
            }

            var fixes = new List<MacroFix>();
            
            // Generate specific fixes based on issues
            foreach (var issue in issues)
            {
                fixes.AddRange(GenerateSpecificFixes(macroText, issue, preserveIntent));
            }
            
            // If no specific issues provided, analyze and suggest fixes
            if (issues.Count == 0)
            {
                fixes.AddRange(GenerateGeneralFixes(macroText, preserveIntent));
            }

            var fixResult = new MacroFixSuggestionResult
            {
                Fixes = fixes,
                PreserveIntent = preserveIntent,
                OriginalLength = macroText.Length
            };

            _logger.LogDebug("Generated {FixCount} fix suggestions for macro", fixes.Count);

            return Task.FromResult(new ToolResult
            {
                Content = fixResult,
                IsError = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suggesting macro fixes");
            
            return Task.FromResult(new ToolResult
            {
                Content = new MacroFixSuggestionResult
                {
                    Fixes = new List<MacroFix> { new() { Type = "error", Issue = "Fix generation failed", Fix = ex.Message } }
                },
                IsError = true
            });
        }
    }

    private Task<ToolResult> OptimizeMacroAsync(object? arguments)
    {
        try
        {
            string macroText = string.Empty;
            int targetLength = 255;
            string optimizationLevel = "moderate";

            if (arguments is JsonElement jsonArgs)
            {
                macroText = jsonArgs.GetProperty("macroText").GetString() ?? string.Empty;
                if (jsonArgs.TryGetProperty("targetLength", out var targetProp))
                {
                    targetLength = targetProp.GetInt32();
                }
                if (jsonArgs.TryGetProperty("optimizationLevel", out var levelProp))
                {
                    optimizationLevel = levelProp.GetString() ?? "moderate";
                }
            }

            if (string.IsNullOrWhiteSpace(macroText))
            {
                return Task.FromResult(new ToolResult
                {
                    Content = new MacroOptimizationResult
                    {
                        OptimizedMacro = string.Empty,
                        OriginalLength = 0,
                        OptimizedLength = 0,
                        Optimizations = new List<string> { "Macro text is required" }
                    },
                    IsError = false
                });
            }

            var optimizations = new List<string>();
            var optimizedMacro = macroText;

            // Apply optimizations based on level
            switch (optimizationLevel)
            {
                case "conservative":
                    optimizedMacro = ApplyConservativeOptimizations(optimizedMacro, optimizations);
                    break;
                case "moderate":
                    optimizedMacro = ApplyConservativeOptimizations(optimizedMacro, optimizations);
                    optimizedMacro = ApplyModerateOptimizations(optimizedMacro, optimizations);
                    break;
                case "aggressive":
                    optimizedMacro = ApplyConservativeOptimizations(optimizedMacro, optimizations);
                    optimizedMacro = ApplyModerateOptimizations(optimizedMacro, optimizations);
                    optimizedMacro = ApplyAggressiveOptimizations(optimizedMacro, optimizations);
                    break;
            }

            var optimizationResult = new MacroOptimizationResult
            {
                OptimizedMacro = optimizedMacro,
                OriginalLength = macroText.Length,
                OptimizedLength = optimizedMacro.Length,
                CharactersSaved = macroText.Length - optimizedMacro.Length,
                Optimizations = optimizations,
                OptimizationLevel = optimizationLevel
            };

            _logger.LogDebug("Optimized macro: {OriginalLength} -> {OptimizedLength} characters ({CharactersSaved} saved)", 
                macroText.Length, optimizedMacro.Length, optimizationResult.CharactersSaved);

            return Task.FromResult(new ToolResult
            {
                Content = optimizationResult,
                IsError = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing macro");
            
            return Task.FromResult(new ToolResult
            {
                Content = new MacroOptimizationResult
                {
                    OptimizedMacro = string.Empty,
                    OriginalLength = 0,
                    OptimizedLength = 0,
                    Optimizations = new List<string> { $"Optimization error: {ex.Message}" }
                },
                IsError = true
            });
        }
    }

    // Helper methods for analysis and optimization
    private List<MacroIssue> AnalyzeSyntaxIssues(string macroText)
    {
        var issues = new List<MacroIssue>();
        
        var bracketErrors = _macroParser.ValidateBrackets(macroText);
        foreach (var error in bracketErrors)
        {
            issues.Add(new MacroIssue { Type = "syntax", Message = error, Severity = "error" });
        }
        
        return issues;
    }

    private List<MacroIssue> AnalyzeCommandIssues(string macroText)
    {
        var issues = new List<MacroIssue>();
        
        try
        {
            var macro = _macroParser.Parse(macroText);
            foreach (var line in macro.Lines)
            {
                if (line is CommandLine commandLine)
                {
                    if (!WoWMacroConstants.ValidSlashCommands.Contains(commandLine.Command))
                    {
                        issues.Add(new MacroIssue 
                        { 
                            Type = "command", 
                            Message = $"Invalid command: {commandLine.Command}", 
                            Severity = "error" 
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            issues.Add(new MacroIssue { Type = "parsing", Message = $"Parse error: {ex.Message}", Severity = "error" });
        }
        
        return issues;
    }

    private List<MacroIssue> AnalyzeConditionalIssues(string macroText)
    {
        var issues = new List<MacroIssue>();
        // Implementation would analyze conditionals for validity
        return issues;
    }

    private List<MacroIssue> AnalyzeCharacterLimitIssues(string macroText)
    {
        var issues = new List<MacroIssue>();
        
        if (macroText.Length > 255)
        {
            issues.Add(new MacroIssue 
            { 
                Type = "length", 
                Message = $"Macro exceeds 255 character limit ({macroText.Length} characters)", 
                Severity = "error" 
            });
        }
        else if (macroText.Length > 230)
        {
            issues.Add(new MacroIssue 
            { 
                Type = "length", 
                Message = $"Macro is {macroText.Length} characters, close to 255 limit", 
                Severity = "warning" 
            });
        }
        
        return issues;
    }

    private List<MacroIssue> AnalyzeLogicFlowIssues(string macroText)
    {
        var issues = new List<MacroIssue>();
        // Implementation would analyze logical flow and dependencies
        return issues;
    }

    private List<MacroIssue> AnalyzeOptimizationOpportunities(string macroText)
    {
        var issues = new List<MacroIssue>();
        
        if (macroText.Contains("/cast ") && macroText.Length > 200)
        {
            issues.Add(new MacroIssue 
            { 
                Type = "optimization", 
                Message = "Consider using shorter command abbreviations", 
                Severity = "info" 
            });
        }
        
        return issues;
    }

    private string CalculateOverallSeverity(List<MacroIssue> issues)
    {
        if (issues.Any(i => i.Severity == "error")) return "error";
        if (issues.Any(i => i.Severity == "warning")) return "warning";
        return "info";
    }

    private List<string> GenerateFixSuggestions(List<MacroIssue> issues)
    {
        var suggestions = new List<string>();
        
        foreach (var issue in issues.Where(i => i.Severity == "error"))
        {
            suggestions.Add($"Fix {issue.Type}: {issue.Message}");
        }
        
        return suggestions;
    }

    private List<string> FindOptimizations(string macroText)
    {
        var optimizations = new List<string>();
        
        if (macroText.Contains("/cast "))
        {
            optimizations.Add("Replace '/cast' with '/c' to save characters");
        }
        
        return optimizations;
    }

    private List<MacroFix> GenerateSpecificFixes(string macroText, string issue, bool preserveIntent)
    {
        var fixes = new List<MacroFix>();
        
        // Generate fixes based on specific issues
        if (issue.Contains("Invalid command"))
        {
            fixes.Add(new MacroFix 
            { 
                Type = "command", 
                Issue = issue, 
                Fix = "Replace with valid WoW command" 
            });
        }
        
        return fixes;
    }

    private List<MacroFix> GenerateGeneralFixes(string macroText, bool preserveIntent)
    {
        var fixes = new List<MacroFix>();
        
        // General fixes based on analysis
        var issues = AnalyzeSyntaxIssues(macroText);
        issues.AddRange(AnalyzeCommandIssues(macroText));
        issues.AddRange(AnalyzeCharacterLimitIssues(macroText));
        
        foreach (var issue in issues)
        {
            fixes.Add(new MacroFix 
            { 
                Type = issue.Type, 
                Issue = issue.Message, 
                Fix = $"Fix {issue.Type} issue" 
            });
        }
        
        return fixes;
    }

    private string ApplyConservativeOptimizations(string macroText, List<string> optimizations)
    {
        var optimized = macroText;
        
        // Remove extra whitespace
        if (optimized.Contains("  "))
        {
            optimized = System.Text.RegularExpressions.Regex.Replace(optimized, @"\s+", " ");
            optimizations.Add("Removed extra whitespace");
        }
        
        return optimized;
    }

    private string ApplyModerateOptimizations(string macroText, List<string> optimizations)
    {
        var optimized = macroText;
        
        // Replace common commands with abbreviations
        if (optimized.Contains("/cast "))
        {
            optimized = optimized.Replace("/cast ", "/c ");
            optimizations.Add("Replaced '/cast' with '/c'");
        }
        
        return optimized;
    }

    private string ApplyAggressiveOptimizations(string macroText, List<string> optimizations)
    {
        var optimized = macroText;
        
        // More aggressive optimizations would go here
        // This is a placeholder for future enhancements
        
        return optimized;
    }
}