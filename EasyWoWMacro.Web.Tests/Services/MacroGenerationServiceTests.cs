using EasyWoWMacro.MCP.Server.Models;
using EasyWoWMacro.Web.Services;
using EasyWoWMacro.Web.Services.Interfaces;
using EasyWoWMacro.Web.Services.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace EasyWoWMacro.Web.Tests.Services;

public class MacroGenerationServiceTests
{
    private readonly Mock<ILLMService> _mockLlmService;
    private readonly Mock<IMCPClientService> _mockMcpClient;
    private readonly Mock<ILogger<MacroGenerationService>> _mockLogger;
    private readonly MacroGenerationService _service;

    public MacroGenerationServiceTests()
    {
        _mockLlmService = new Mock<ILLMService>();
        _mockMcpClient = new Mock<IMCPClientService>();
        _mockLogger = new Mock<ILogger<MacroGenerationService>>();
        
        _service = new MacroGenerationService(
            _mockLlmService.Object,
            _mockMcpClient.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void IsAvailable_WhenBothServicesAvailable_ShouldReturnTrue()
    {
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        Assert.True(_service.IsAvailable);
    }

    [Fact]
    public void IsAvailable_WhenLLMNotConfigured_ShouldReturnFalse()
    {
        _mockLlmService.Setup(x => x.IsConfigured).Returns(false);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        Assert.False(_service.IsAvailable);
    }

    [Fact]
    public void IsAvailable_WhenMCPNotConnected_ShouldReturnFalse()
    {
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(false);

        Assert.False(_service.IsAvailable);
    }

    [Fact]
    public async Task GenerateFromDescriptionAsync_WhenNotAvailable_ShouldReturnError()
    {
        _mockLlmService.Setup(x => x.IsConfigured).Returns(false);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(false);

        var request = new MacroGenerationRequest { Description = "Test macro" };
        var result = await _service.GenerateFromDescriptionAsync(request);

        Assert.False(result.Success);
        Assert.Contains("not available", result.Errors.First());
    }

    [Fact]
    public async Task GenerateFromDescriptionAsync_WithValidInput_ShouldReturnSuccess()
    {
        // Arrange
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        // Setup MCP resource calls
        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://commands", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResourceResult
            {
                Contents = new[] { "/cast", "/use", "/target" },
                MimeType = "application/json"
            });

        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://conditionals", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResourceResult
            {
                Contents = new[] { "combat", "harm", "help", "@target" },
                MimeType = "application/json"
            });

        // Setup LLM generation
        _mockLlmService.Setup(x => x.GenerateMacroAsync(It.IsAny<MacroGenerationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MacroGenerationResult
            {
                Success = true,
                GeneratedMacro = "/cast Fireball",
                Explanation = "Casts Fireball spell",
                TokensUsed = 50
            });

        // Setup MCP validation
        var validationResult = new MacroValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            Warnings = new List<string>(),
            Suggestions = new List<string>(),
            FormattedMacro = "/cast Fireball",
            CharacterCount = 14
        };

        _mockMcpClient.Setup(x => x.CallToolAsync("validate_macro", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToolResult
            {
                Content = validationResult,
                IsError = false
            });

        var request = new MacroGenerationRequest { Description = "Cast a fireball spell" };

        // Act
        var result = await _service.GenerateFromDescriptionAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("/cast Fireball", result.GeneratedMacro);
        Assert.Equal("Casts Fireball spell", result.Explanation);
        Assert.Equal(14, result.CharacterCount);
        Assert.Equal(50, result.TokensUsed);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GenerateFromDescriptionAsync_WithInvalidMacro_ShouldAttemptFix()
    {
        // Arrange
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        // Setup MCP resource calls
        SetupMockMCPResources();

        // Setup initial LLM generation (invalid macro)
        var invalidMacro = "/invalid_command Something";
        _mockLlmService.SetupSequence(x => x.GenerateMacroAsync(It.IsAny<MacroGenerationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MacroGenerationResult
            {
                Success = true,
                GeneratedMacro = invalidMacro,
                Explanation = "Invalid macro",
                TokensUsed = 30
            })
            .ReturnsAsync(new MacroGenerationResult
            {
                Success = true,
                GeneratedMacro = "/cast Fireball",
                Explanation = "Fixed macro",
                TokensUsed = 35
            });

        // Setup MCP validation (first fails, second succeeds)
        var failedValidation = new MacroValidationResult
        {
            IsValid = false,
            Errors = new List<string> { "Invalid command: /invalid_command" },
            FormattedMacro = invalidMacro,
            CharacterCount = invalidMacro.Length
        };

        var successValidation = new MacroValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            FormattedMacro = "/cast Fireball",
            CharacterCount = 14
        };

        _mockMcpClient.SetupSequence(x => x.CallToolAsync("validate_macro", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToolResult { Content = failedValidation, IsError = false })
            .ReturnsAsync(new ToolResult { Content = successValidation, IsError = false });

        var request = new MacroGenerationRequest { Description = "Cast a fireball spell" };

        // Act
        var result = await _service.GenerateFromDescriptionAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("/cast Fireball", result.GeneratedMacro);
        Assert.Equal("Fixed macro", result.Explanation);
        Assert.Empty(result.Errors);

        // Verify LLM was called twice (initial + fix)
        _mockLlmService.Verify(x => x.GenerateMacroAsync(It.IsAny<MacroGenerationRequest>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Fact]
    public async Task ValidateAndOptimizeAsync_WithValidMacro_ShouldReturnValidation()
    {
        // Arrange
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        var validationResult = new MacroValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            Warnings = new List<string> { "Macro is close to character limit" },
            Suggestions = new List<string> { "Consider using shorter spell names" },
            FormattedMacro = "/cast Fireball",
            CharacterCount = 14
        };

        _mockMcpClient.Setup(x => x.CallToolAsync("validate_macro", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToolResult
            {
                Content = validationResult,
                IsError = false
            });

        // Act
        var result = await _service.ValidateAndOptimizeAsync("/cast Fireball");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("/cast Fireball", result.GeneratedMacro);
        Assert.Empty(result.Errors);
        Assert.Single(result.Warnings);
        Assert.Single(result.Suggestions);
        Assert.Equal(14, result.CharacterCount);
    }

    [Fact]
    public async Task ValidateAndOptimizeAsync_WithInvalidMacro_ShouldReturnErrors()
    {
        // Arrange
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        var validationResult = new MacroValidationResult
        {
            IsValid = false,
            Errors = new List<string> { "Invalid command: /invalid_command" },
            Warnings = new List<string>(),
            Suggestions = new List<string>(),
            FormattedMacro = "/invalid_command Something",
            CharacterCount = 25
        };

        _mockMcpClient.Setup(x => x.CallToolAsync("validate_macro", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToolResult
            {
                Content = validationResult,
                IsError = false
            });

        // Act
        var result = await _service.ValidateAndOptimizeAsync("/invalid_command Something");

        // Assert
        Assert.False(result.Success);
        Assert.Single(result.Errors);
        Assert.Contains("Invalid command", result.Errors.First());
        Assert.Equal(25, result.CharacterCount);
    }

    [Fact]
    public async Task GenerateFromDescriptionAsync_WhenMCPResourcesFail_ShouldUseFallback()
    {
        // Arrange
        _mockLlmService.Setup(x => x.IsConfigured).Returns(true);
        _mockMcpClient.Setup(x => x.IsConnected).Returns(true);

        // Setup MCP resource calls to fail
        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://commands", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MCP resource error"));

        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://conditionals", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MCP resource error"));

        // Setup LLM generation
        _mockLlmService.Setup(x => x.GenerateMacroAsync(It.IsAny<MacroGenerationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MacroGenerationResult
            {
                Success = true,
                GeneratedMacro = "/cast Fireball",
                Explanation = "Casts Fireball spell",
                TokensUsed = 50
            });

        // Setup MCP validation
        var validationResult = new MacroValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            FormattedMacro = "/cast Fireball",
            CharacterCount = 14
        };

        _mockMcpClient.Setup(x => x.CallToolAsync("validate_macro", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ToolResult { Content = validationResult, IsError = false });

        var request = new MacroGenerationRequest { Description = "Cast a fireball spell" };

        // Act
        var result = await _service.GenerateFromDescriptionAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("/cast Fireball", result.GeneratedMacro);
        
        // Verify LLM was still called despite MCP resource failures
        _mockLlmService.Verify(x => x.GenerateMacroAsync(It.IsAny<MacroGenerationRequest>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    private void SetupMockMCPResources()
    {
        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://commands", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResourceResult
            {
                Contents = new[] { "/cast", "/use", "/target" },
                MimeType = "application/json"
            });

        _mockMcpClient.Setup(x => x.ReadResourceAsync("wow://conditionals", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResourceResult
            {
                Contents = new[] { "combat", "harm", "help", "@target" },
                MimeType = "application/json"
            });
    }
}