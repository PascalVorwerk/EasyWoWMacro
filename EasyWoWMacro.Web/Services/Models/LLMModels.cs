using System.Text.Json.Serialization;

namespace EasyWoWMacro.Web.Services.Models;

public record LLMRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;
    
    [JsonPropertyName("messages")]
    public List<LLMMessage> Messages { get; init; } = new();
    
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 1000;
    
    [JsonPropertyName("temperature")]
    public double Temperature { get; init; } = 0.7;
    
    [JsonPropertyName("tools")]
    public List<LLMTool>? Tools { get; init; }
    
    [JsonPropertyName("tool_choice")]
    public string? ToolChoice { get; init; }
}

public record LLMMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
    
    [JsonPropertyName("tool_calls")]
    public List<LLMToolCall>? ToolCalls { get; init; }
}

public record LLMTool
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";
    
    [JsonPropertyName("function")]
    public LLMFunction Function { get; init; } = new();
}

public record LLMFunction
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    [JsonPropertyName("parameters")]
    public object Parameters { get; init; } = new { };
}

public record LLMToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";
    
    [JsonPropertyName("function")]
    public LLMFunctionCall Function { get; init; } = new();
}

public record LLMFunctionCall
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; init; } = string.Empty;
}

public record LLMResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("choices")]
    public List<LLMChoice> Choices { get; init; } = new();
    
    [JsonPropertyName("usage")]
    public LLMUsage? Usage { get; init; }
    
    [JsonPropertyName("error")]
    public LLMError? Error { get; init; }
}

public record LLMChoice
{
    [JsonPropertyName("index")]
    public int Index { get; init; }
    
    [JsonPropertyName("message")]
    public LLMMessage Message { get; init; } = new();
    
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

public record LLMUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }
    
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }
    
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}

public record LLMError
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
    
    [JsonPropertyName("code")]
    public string? Code { get; init; }
}

public record MacroGenerationRequest
{
    public string Description { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public bool IsPremuimUser { get; init; } = false;
}

public record MacroGenerationResult
{
    public bool Success { get; init; }
    public string? GeneratedMacro { get; init; }
    public string? Explanation { get; init; }
    public List<string> Errors { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<string> Suggestions { get; init; } = new();
    public int CharacterCount { get; init; }
    public int TokensUsed { get; init; }
    public TimeSpan ProcessingTime { get; init; }
}