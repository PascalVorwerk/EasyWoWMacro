using EasyWoWMacro.MCP.Server.Models;
using EasyWoWMacro.Web.Services.Interfaces;
using EasyWoWMacro.Web.Services.Models;
using System.Text.Json;

namespace EasyWoWMacro.Web.Services;

/// <summary>
/// Service that combines LLM capabilities with MCP validation tools to generate high-quality WoW macros
/// </summary>
public class MacroGenerationService : IMacroGenerationService
{
    private readonly ILLMService _llmService;
    private readonly IMCPClientService _mcpClient;
    private readonly ILogger<MacroGenerationService> _logger;

    public MacroGenerationService(
        ILLMService llmService,
        IMCPClientService mcpClient,
        ILogger<MacroGenerationService> logger)
    {
        _llmService = llmService;
        _mcpClient = mcpClient;
        _logger = logger;
    }

    public bool IsAvailable => _llmService.IsConfigured && _mcpClient.IsConnected;

    public async Task<MacroGenerationResult> GenerateFromDescriptionAsync(MacroGenerationRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { "Macro generation service is not available" }
            };
        }

        var startTime = DateTime.UtcNow;
        
        try
        {
            _logger.LogInformation("Generating macro for description: {Description}", request.Description);

            // Step 1: Get available commands and conditionals from MCP
            var (commands, conditionals) = await GetWoWReferenceDataAsync(cancellationToken);

            // Step 2: Generate initial macro using LLM with enhanced context
            var enhancedRequest = await CreateEnhancedLLMRequestAsync(request, commands, conditionals, cancellationToken);
            var llmResult = await _llmService.GenerateMacroAsync(enhancedRequest, cancellationToken);

            if (!llmResult.Success || string.IsNullOrEmpty(llmResult.GeneratedMacro))
            {
                return new MacroGenerationResult
                {
                    Success = false,
                    Errors = llmResult.Errors,
                    ProcessingTime = DateTime.UtcNow - startTime
                };
            }

            // Step 3: Validate and refine the generated macro using MCP tools
            var validationResult = await ValidateWithMCPAsync(llmResult.GeneratedMacro, cancellationToken);
            
            // Step 4: If validation fails, attempt to fix the macro
            if (!validationResult.IsValid && validationResult.Errors.Any())
            {
                _logger.LogDebug("Initial macro validation failed, attempting to fix: {Errors}", 
                    string.Join(", ", validationResult.Errors));
                
                var fixedResult = await AttemptMacroFixAsync(llmResult.GeneratedMacro, validationResult, request, cancellationToken);
                if (fixedResult.Success)
                {
                    llmResult = fixedResult;
                    validationResult = await ValidateWithMCPAsync(fixedResult.GeneratedMacro!, cancellationToken);
                }
            }

            var processingTime = DateTime.UtcNow - startTime;

            return new MacroGenerationResult
            {
                Success = validationResult.IsValid,
                GeneratedMacro = llmResult.GeneratedMacro,
                Explanation = llmResult.Explanation,
                Errors = validationResult.Errors,
                Warnings = validationResult.Warnings,
                Suggestions = validationResult.Suggestions,
                CharacterCount = validationResult.CharacterCount,
                TokensUsed = llmResult.TokensUsed,
                ProcessingTime = processingTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating macro for description: {Description}", request.Description);
            
            return new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { $"Error generating macro: {ex.Message}" },
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }
    }

    public async Task<MacroGenerationResult> ValidateAndOptimizeAsync(string macroText, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { "Macro validation service is not available" }
            };
        }

        var startTime = DateTime.UtcNow;

        try
        {
            var validationResult = await ValidateWithMCPAsync(macroText, cancellationToken);
            
            return new MacroGenerationResult
            {
                Success = validationResult.IsValid,
                GeneratedMacro = validationResult.FormattedMacro,
                Errors = validationResult.Errors,
                Warnings = validationResult.Warnings,
                Suggestions = validationResult.Suggestions,
                CharacterCount = validationResult.CharacterCount,
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating macro: {MacroText}", macroText);
            
            return new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { $"Error validating macro: {ex.Message}" },
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }
    }

    private async Task<(string[] commands, string[] conditionals)> GetWoWReferenceDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var commandsTask = _mcpClient.ReadResourceAsync("wow://commands", cancellationToken);
            var conditionalsTask = _mcpClient.ReadResourceAsync("wow://conditionals", cancellationToken);

            await Task.WhenAll(commandsTask, conditionalsTask);

            var commands = commandsTask.Result.Contents as string[] ?? Array.Empty<string>();
            var conditionals = conditionalsTask.Result.Contents as string[] ?? Array.Empty<string>();

            return (commands, conditionals);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get WoW reference data from MCP, using fallback");
            
            // Fallback to basic commands and conditionals
            return (
                new[] { "/cast", "/use", "/target", "/focus", "/cancelaura" },
                new[] { "combat", "harm", "help", "mod:shift", "@target", "@mouseover" }
            );
        }
    }

    private async Task<MacroGenerationRequest> CreateEnhancedLLMRequestAsync(
        MacroGenerationRequest request, 
        string[] commands, 
        string[] conditionals, 
        CancellationToken cancellationToken)
    {
        // Enhance the description with available commands and conditionals
        var enhancedDescription = $"""
{request.Description}

Available Commands: {string.Join(", ", commands.Take(20))}
Available Conditionals: {string.Join(", ", conditionals.Take(20))}
""";

        return new MacroGenerationRequest
        {
            Description = enhancedDescription,
            UserId = request.UserId,
            IsPremuimUser = request.IsPremuimUser
        };
    }

    private async Task<MacroValidationResult> ValidateWithMCPAsync(string macroText, CancellationToken cancellationToken)
    {
        try
        {
            var arguments = JsonSerializer.SerializeToElement(new { macroText });
            var result = await _mcpClient.CallToolAsync("validate_macro", arguments, cancellationToken);

            if (result.IsError)
            {
                return new MacroValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "MCP validation failed" }
                };
            }

            var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
                JsonSerializer.Serialize(result.Content));

            return validationResult ?? new MacroValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Failed to parse validation result" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating macro with MCP: {MacroText}", macroText);
            
            return new MacroValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Validation error: {ex.Message}" }
            };
        }
    }

    private async Task<MacroGenerationResult> AttemptMacroFixAsync(
        string originalMacro, 
        MacroValidationResult validationResult, 
        MacroGenerationRequest originalRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var fixPrompt = $"""
The following WoW macro has validation errors. Please fix it:

ORIGINAL MACRO: {originalMacro}
ERRORS: {string.Join(", ", validationResult.Errors)}
ORIGINAL REQUEST: {originalRequest.Description}

Please provide a corrected version that addresses the validation errors while maintaining the original intent.
""";

            var fixRequest = new MacroGenerationRequest
            {
                Description = fixPrompt,
                UserId = originalRequest.UserId,
                IsPremuimUser = originalRequest.IsPremuimUser
            };

            var fixResult = await _llmService.GenerateMacroAsync(fixRequest, cancellationToken);
            
            _logger.LogDebug("Attempted macro fix: {Success}", fixResult.Success);
            
            return fixResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error attempting to fix macro");
            
            return new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { $"Error fixing macro: {ex.Message}" }
            };
        }
    }
}