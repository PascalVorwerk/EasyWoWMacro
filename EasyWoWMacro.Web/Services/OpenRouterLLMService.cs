using EasyWoWMacro.Web.Services.Interfaces;
using EasyWoWMacro.Web.Services.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EasyWoWMacro.Web.Services;

public class OpenRouterLLMService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouterOptions _options;
    private readonly ILogger<OpenRouterLLMService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public OpenRouterLLMService(
        HttpClient httpClient,
        IOptions<OpenRouterOptions> options,
        ILogger<OpenRouterLLMService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        
        ConfigureHttpClient();
    }

    public bool IsConfigured => !string.IsNullOrEmpty(_options.ApiKey);

    public async Task<LLMResponse> SendRequestAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("OpenRouter API key not configured");
        }

        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending LLM request to OpenRouter: {Model}, {MessageCount} messages", 
                request.Model, request.Messages.Count);

            var response = await _httpClient.PostAsync("/api/v1/chat/completions", content, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenRouter API error: {StatusCode} - {Response}", 
                    response.StatusCode, responseJson);
                
                return new LLMResponse
                {
                    Error = new LLMError
                    {
                        Message = $"OpenRouter API error: {response.StatusCode}",
                        Type = "api_error",
                        Code = response.StatusCode.ToString()
                    }
                };
            }

            var llmResponse = JsonSerializer.Deserialize<LLMResponse>(responseJson, _jsonOptions);
            
            _logger.LogDebug("Received LLM response: {ChoiceCount} choices, {TotalTokens} tokens", 
                llmResponse?.Choices.Count ?? 0, llmResponse?.Usage?.TotalTokens ?? 0);

            return llmResponse ?? new LLMResponse
            {
                Error = new LLMError { Message = "Failed to deserialize response", Type = "parse_error" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending request to OpenRouter");
            
            return new LLMResponse
            {
                Error = new LLMError
                {
                    Message = ex.Message,
                    Type = "client_error"
                }
            };
        }
    }

    public async Task<MacroGenerationResult> GenerateMacroAsync(MacroGenerationRequest request, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var systemPrompt = CreateSystemPrompt();
            var userPrompt = CreateUserPrompt(request.Description);

            var llmRequest = new LLMRequest
            {
                Model = _options.Model,
                Messages = new List<LLMMessage>
                {
                    new() { Role = "system", Content = systemPrompt },
                    new() { Role = "user", Content = userPrompt }
                },
                MaxTokens = 1000,
                Temperature = 0.3 // Lower temperature for more consistent macro generation
            };

            var response = await SendRequestAsync(llmRequest, cancellationToken);
            var processingTime = DateTime.UtcNow - startTime;

            if (response.Error != null)
            {
                return new MacroGenerationResult
                {
                    Success = false,
                    Errors = new List<string> { response.Error.Message },
                    ProcessingTime = processingTime
                };
            }

            var choice = response.Choices.FirstOrDefault();
            if (choice?.Message?.Content == null)
            {
                return new MacroGenerationResult
                {
                    Success = false,
                    Errors = new List<string> { "No response content received from LLM" },
                    ProcessingTime = processingTime
                };
            }

            var generatedContent = choice.Message.Content;
            var (macro, explanation) = ParseGeneratedContent(generatedContent);

            return new MacroGenerationResult
            {
                Success = true,
                GeneratedMacro = macro,
                Explanation = explanation,
                CharacterCount = macro?.Length ?? 0,
                TokensUsed = response.Usage?.TotalTokens ?? 0,
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

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _options.AppName);
        _httpClient.DefaultRequestHeaders.Add("X-Title", _options.AppName);
    }

    private string CreateSystemPrompt()
    {
        return """
You are a World of Warcraft macro expert. Your task is to convert natural language descriptions into valid WoW macros.

CRITICAL RULES:
1. WoW macros have a 255 character limit - this is MANDATORY
2. Use only valid WoW slash commands and conditionals
3. Follow exact WoW macro syntax rules
4. Always validate your output fits within 255 characters

VALID COMMANDS (examples):
/cast, /use, /target, /focus, /assist, /cancelaura, /stopcasting, /dismount

VALID CONDITIONALS (examples):
- [@target,harm] - target exists and is hostile
- [@target,help] - target exists and is friendly  
- [mod:shift] - shift key is held
- [combat] - in combat
- [nocombat] - not in combat
- [@mouseover] - mouseover target
- [@player] - target yourself

CONDITIONAL SYNTAX:
- Use comma for AND: [mod:shift,combat] = shift AND in combat
- Use semicolon for OR: [mod:shift;mod:ctrl] = shift OR ctrl
- Use semicolon to separate clauses: /cast [harm] Spell1; [help] Spell2; Spell3

OUTPUT FORMAT:
MACRO:
[your generated macro here]

EXPLANATION:
[brief explanation of what the macro does]

Be concise and ensure the macro works in WoW.
""";
    }

    private string CreateUserPrompt(string description)
    {
        return $"Create a WoW macro for: {description}";
    }

    private (string? macro, string? explanation) ParseGeneratedContent(string content)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string? macro = null;
        string? explanation = null;
        bool inMacroSection = false;
        bool inExplanationSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.Equals("MACRO:", StringComparison.OrdinalIgnoreCase))
            {
                inMacroSection = true;
                inExplanationSection = false;
                continue;
            }
            
            if (trimmedLine.Equals("EXPLANATION:", StringComparison.OrdinalIgnoreCase))
            {
                inMacroSection = false;
                inExplanationSection = true;
                continue;
            }

            if (inMacroSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                macro = trimmedLine;
            }
            else if (inExplanationSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                explanation = string.IsNullOrEmpty(explanation) ? trimmedLine : $"{explanation} {trimmedLine}";
            }
        }

        // Fallback: if no structured format, try to extract macro from content
        if (string.IsNullOrEmpty(macro))
        {
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("/") && trimmedLine.Length > 1)
                {
                    macro = trimmedLine;
                    break;
                }
            }
        }

        return (macro, explanation);
    }
}

public class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://openrouter.ai";
    public string Model { get; set; } = "deepseek/deepseek-chat-v3-0324:free";
    public string AppName { get; set; } = "EasyWoWMacro";
}