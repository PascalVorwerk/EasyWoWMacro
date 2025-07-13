using EasyWoWMacro.MCP.Server.Models;

namespace EasyWoWMacro.MCP.Server.Interfaces;

public interface IMCPServer
{
    Task<MCPResponse> HandleRequestAsync(MCPRequest request);
    Task<ServerInfo> GetServerInfoAsync();
    Task<List<MCPTool>> ListToolsAsync();
    Task<List<MCPResource>> ListResourcesAsync();
    Task<ToolResult> CallToolAsync(string toolName, object? arguments);
    Task<ResourceResult> ReadResourceAsync(string uri);
    bool IsHealthy { get; }
}