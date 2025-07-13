using EasyWoWMacro.MCP.Server;
using EasyWoWMacro.MCP.Server.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace EasyWoWMacro.MCP.Server.Tests;

public class WoWMacroMCPServerTests
{
    private readonly Mock<ILogger<WoWMacroMCPServer>> _mockLogger;
    private readonly WoWMacroMCPServer _server;

    public WoWMacroMCPServerTests()
    {
        _mockLogger = new Mock<ILogger<WoWMacroMCPServer>>();
        _server = new WoWMacroMCPServer(_mockLogger.Object);
    }

    [Fact]
    public void Server_ShouldBeHealthy_WhenInitialized()
    {
        Assert.True(_server.IsHealthy);
    }

    [Fact]
    public async Task GetServerInfoAsync_ShouldReturnValidServerInfo()
    {
        var serverInfo = await _server.GetServerInfoAsync();

        Assert.NotNull(serverInfo);
        Assert.Equal("WoW Macro MCP Server", serverInfo.Name);
        Assert.Equal("1.0.0", serverInfo.Version);
        Assert.Equal("2025-06-18", serverInfo.ProtocolVersion);
    }


    [Fact]
    public async Task ListResourcesAsync_ShouldReturnCommandsAndConditionals()
    {
        var resources = await _server.ListResourcesAsync();

        Assert.NotNull(resources);
        Assert.Equal(2, resources.Count);

        var commandsResource = resources.FirstOrDefault(r => r.Uri == "wow://commands");
        var conditionalsResource = resources.FirstOrDefault(r => r.Uri == "wow://conditionals");

        Assert.NotNull(commandsResource);
        Assert.NotNull(conditionalsResource);
        Assert.Equal("WoW Commands", commandsResource.Name);
        Assert.Equal("WoW Conditionals", conditionalsResource.Name);
    }

    [Fact]
    public async Task ReadResourceAsync_Commands_ShouldReturnValidCommands()
    {
        var result = await _server.ReadResourceAsync("wow://commands");

        Assert.NotNull(result);
        Assert.Equal("application/json", result.MimeType);
        Assert.NotNull(result.Contents);

        var commands = result.Contents as string[];
        Assert.NotNull(commands);
        Assert.Contains("/cast", commands);
        Assert.Contains("/target", commands);
    }

    [Fact]
    public async Task ReadResourceAsync_Conditionals_ShouldReturnValidConditionals()
    {
        var result = await _server.ReadResourceAsync("wow://conditionals");

        Assert.NotNull(result);
        Assert.Equal("application/json", result.MimeType);
        Assert.NotNull(result.Contents);

        var conditionals = result.Contents as string[];
        Assert.NotNull(conditionals);
        Assert.Contains("combat", conditionals);
        Assert.Contains("mod", conditionals);
    }

    [Fact]
    public async Task ValidateMacro_ValidMacro_ShouldReturnSuccess()
    {
        var arguments = JsonSerializer.SerializeToElement(new { macroText = "/cast Fireball" });

        var result = await _server.CallToolAsync("validate_macro", arguments);

        Assert.NotNull(result);
        Assert.False(result.IsError);

        var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
            JsonSerializer.Serialize(result.Content));

        Assert.NotNull(validationResult);
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
        Assert.NotNull(validationResult.FormattedMacro);
        Assert.True(validationResult.CharacterCount > 0);
    }

    [Fact]
    public async Task ValidateMacro_EmptyMacro_ShouldReturnError()
    {
        var arguments = JsonSerializer.SerializeToElement(new { macroText = "" });

        var result = await _server.CallToolAsync("validate_macro", arguments);

        Assert.NotNull(result);
        Assert.False(result.IsError);

        var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
            JsonSerializer.Serialize(result.Content));

        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.Contains("Macro text is required", validationResult.Errors);
    }

    [Fact]
    public async Task ValidateMacro_TooLongMacro_ShouldReturnError()
    {
        var longMacro = "/cast " + new string('A', 300);
        var arguments = JsonSerializer.SerializeToElement(new { macroText = longMacro });

        var result = await _server.CallToolAsync("validate_macro", arguments);

        Assert.NotNull(result);
        Assert.False(result.IsError);

        var validationResult = JsonSerializer.Deserialize<MacroValidationResult>(
            JsonSerializer.Serialize(result.Content));

        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.Contains("exceeds 255 character limit", validationResult.Errors[0]);
    }

    [Fact]
    public async Task HandleRequestAsync_InitializeMethod_ShouldReturnServerInfo()
    {
        var request = new MCPRequest
        {
            Id = "test-1",
            Method = "initialize"
        };

        var response = await _server.HandleRequestAsync(request);

        Assert.NotNull(response);
        Assert.Equal("test-1", response.Id);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);

        var serverInfo = JsonSerializer.Deserialize<ServerInfo>(
            JsonSerializer.Serialize(response.Result));
        Assert.NotNull(serverInfo);
        Assert.Equal("WoW Macro MCP Server", serverInfo.Name);
    }

    [Fact]
    public async Task HandleRequestAsync_UnknownMethod_ShouldReturnError()
    {
        var request = new MCPRequest
        {
            Id = "test-2",
            Method = "unknown_method"
        };

        var response = await _server.HandleRequestAsync(request);

        Assert.NotNull(response);
        Assert.Equal("test-2", response.Id);
        Assert.NotNull(response.Error);
        Assert.Equal(-32603, response.Error.Code);
        Assert.Equal("Internal error", response.Error.Message);
    }
}