using EasyWoWMacro.MCP.Server.Models;

namespace EasyWoWMacro.Web.Services.Interfaces;

public interface IMCPClientService
{
    Task<MCPResponse> SendRequestAsync(MCPRequest request, CancellationToken cancellationToken = default);
    Task<List<MCPTool>> ListToolsAsync(CancellationToken cancellationToken = default);
    Task<List<MCPResource>> ListResourcesAsync(CancellationToken cancellationToken = default);
    Task<ToolResult> CallToolAsync(string toolName, object? arguments, CancellationToken cancellationToken = default);
    Task<ResourceResult> ReadResourceAsync(string uri, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}