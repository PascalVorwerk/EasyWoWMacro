using System.Text.Json.Serialization;

namespace EasyWoWMacro.MCP.Server.Models;

public record MCPRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";
    
    [JsonPropertyName("id")]
    public string? Id { get; init; }
    
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;
    
    [JsonPropertyName("params")]
    public object? Params { get; init; }
}

public record MCPResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";
    
    [JsonPropertyName("id")]
    public string? Id { get; init; }
    
    [JsonPropertyName("result")]
    public object? Result { get; init; }
    
    [JsonPropertyName("error")]
    public MCPError? Error { get; init; }
}

public record MCPError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
    
    [JsonPropertyName("data")]
    public object? Data { get; init; }
}

public record MCPTool
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    [JsonPropertyName("inputSchema")]
    public object InputSchema { get; init; } = new { };
}

public record MCPResource
{
    [JsonPropertyName("uri")]
    public string Uri { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    [JsonPropertyName("mimeType")]
    public string MimeType { get; init; } = "text/plain";
}

public record ToolResult
{
    [JsonPropertyName("content")]
    public object Content { get; init; } = new { };
    
    [JsonPropertyName("isError")]
    public bool IsError { get; init; }
}

public record ResourceResult
{
    [JsonPropertyName("contents")]
    public object Contents { get; init; } = new { };
    
    [JsonPropertyName("mimeType")]
    public string MimeType { get; init; } = "text/plain";
}

public record ServerInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;
    
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; init; } = "2025-06-18";
}

public record MacroValidationResult
{
    [JsonPropertyName("isValid")]
    public bool IsValid { get; init; }
    
    [JsonPropertyName("errors")]
    public List<string> Errors { get; init; } = new();
    
    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; init; } = new();
    
    [JsonPropertyName("suggestions")]
    public List<string> Suggestions { get; init; } = new();
    
    [JsonPropertyName("formattedMacro")]
    public string? FormattedMacro { get; init; }
    
    [JsonPropertyName("characterCount")]
    public int CharacterCount { get; init; }
}