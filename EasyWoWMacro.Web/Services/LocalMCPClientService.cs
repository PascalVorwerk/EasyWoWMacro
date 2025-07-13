using EasyWoWMacro.MCP.Server;
using EasyWoWMacro.MCP.Server.Interfaces;
using EasyWoWMacro.MCP.Server.Models;
using EasyWoWMacro.Web.Services.Interfaces;

namespace EasyWoWMacro.Web.Services;

/// <summary>
/// Local MCP client that directly communicates with the WoW Macro MCP Server
/// This is more efficient than HTTP for local operations and simplifies testing
/// </summary>
public class LocalMCPClientService : IMCPClientService
{
    private readonly IMCPServer _mcpServer;
    private readonly ILogger<LocalMCPClientService> _logger;

    public LocalMCPClientService(IMCPServer mcpServer, ILogger<LocalMCPClientService> logger)
    {
        _mcpServer = mcpServer;
        _logger = logger;
    }

    public bool IsConnected => _mcpServer.IsHealthy;

    public async Task<MCPResponse> SendRequestAsync(MCPRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Sending MCP request: {Method}", request.Method);
            var response = await _mcpServer.HandleRequestAsync(request);
            _logger.LogDebug("Received MCP response for request {Id}", request.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending MCP request: {Method}", request.Method);
            
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

    public async Task<List<MCPTool>> ListToolsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Listing MCP tools");
            var tools = await _mcpServer.ListToolsAsync();
            _logger.LogDebug("Found {Count} MCP tools", tools.Count);
            return tools;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing MCP tools");
            return new List<MCPTool>();
        }
    }

    public async Task<List<MCPResource>> ListResourcesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Listing MCP resources");
            var resources = await _mcpServer.ListResourcesAsync();
            _logger.LogDebug("Found {Count} MCP resources", resources.Count);
            return resources;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing MCP resources");
            return new List<MCPResource>();
        }
    }

    public async Task<ToolResult> CallToolAsync(string toolName, object? arguments, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Calling MCP tool: {ToolName}", toolName);
            var result = await _mcpServer.CallToolAsync(toolName, arguments);
            _logger.LogDebug("MCP tool {ToolName} returned {IsError}", toolName, result.IsError ? "error" : "success");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling MCP tool: {ToolName}", toolName);
            
            return new ToolResult
            {
                Content = new { error = ex.Message },
                IsError = true
            };
        }
    }

    public async Task<ResourceResult> ReadResourceAsync(string uri, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Reading MCP resource: {Uri}", uri);
            var result = await _mcpServer.ReadResourceAsync(uri);
            _logger.LogDebug("Read MCP resource: {Uri}", uri);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading MCP resource: {Uri}", uri);
            throw;
        }
    }
}