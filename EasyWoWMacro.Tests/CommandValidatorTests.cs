using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Tests;

public class CommandValidatorTests
{
    [Fact]
    public void IsValidCommand_ValidCommand_ShouldReturnTrue()
    {
        // Arrange
        var command = "/cast";

        // Act
        var isValid = CommandValidator.IsValidCommand(command);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidCommand_InvalidCommand_ShouldReturnFalse()
    {
        // Arrange
        var command = "/invalidcommand";

        // Act
        var isValid = CommandValidator.IsValidCommand(command);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValidCommand_EmptyCommand_ShouldReturnFalse()
    {
        // Arrange
        var command = "";

        // Act
        var isValid = CommandValidator.IsValidCommand(command);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateCommandLine_ValidCommand_ShouldReturnNoErrors()
    {
        // Arrange
        var command = new CommandLine 
        { 
            Command = "/cast",
            Arguments = new List<CommandArgument>
            {
                new() { Value = "Fireball" }
            }
        };

        // Act
        var errors = CommandValidator.ValidateCommandLine(command);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateCommandLine_InvalidCommand_ShouldReturnErrors()
    {
        // Arrange
        var command = new CommandLine 
        { 
            Command = "/invalidcommand",
            Arguments = new List<CommandArgument>
            {
                new() { Value = "Fireball" }
            }
        };

        // Act
        var errors = CommandValidator.ValidateCommandLine(command);

        // Assert
        Assert.Single(errors);
        Assert.Contains("Invalid command", errors[0]);
    }

    [Fact]
    public void GetValidCommands_ShouldReturnCommands()
    {
        // Act
        var commands = CommandValidator.GetValidCommands().ToList();

        // Assert
        Assert.NotEmpty(commands);
        Assert.Contains("/cast", commands);
        Assert.Contains("/use", commands);
        Assert.Contains("/target", commands);
        Assert.Contains("/focus", commands);
        Assert.Contains("/assist", commands);
        Assert.Contains("/stopmacro", commands);
        Assert.Contains("/showtooltip", commands);
    }

    [Theory]
    [InlineData("/cast")]
    [InlineData("/use")]
    [InlineData("/target")]
    [InlineData("/focus")]
    [InlineData("/assist")]
    [InlineData("/stopmacro")]
    [InlineData("/showtooltip")]
    [InlineData("/castsequence")]
    [InlineData("/cancelaura")]
    [InlineData("/cancelform")]
    [InlineData("/cleartarget")]
    [InlineData("/dismount")]
    [InlineData("/equip")]
    [InlineData("/equipslot")]
    [InlineData("/petattack")]
    [InlineData("/petfollow")]
    [InlineData("/petstay")]
    [InlineData("/petpassive")]
    [InlineData("/petdefensive")]
    [InlineData("/petaggressive")]
    public void IsValidCommand_VariousValidCommands_ShouldReturnTrue(string commandName)
    {
        // Arrange
        var command = commandName;

        // Act
        var isValid = CommandValidator.IsValidCommand(command);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("invalidcommand")]
    [InlineData("castinvalid")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("CAST")]
    [InlineData("Cast")]
    public void IsValidCommand_VariousInvalidCommands_ShouldReturnFalse(string commandName)
    {
        // Arrange
        var command = commandName;

        // Act
        var isValid = CommandValidator.IsValidCommand(command);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateCommandLine_CommandWithMultipleArguments_ShouldReturnNoErrors()
    {
        // Arrange
        var command = new CommandLine 
        { 
            Command = "/castsequence",
            Arguments = new List<CommandArgument>
            {
                new() { Value = "Fireball" },
                new() { Value = "Frostbolt" },
                new() { Value = "Arcane Missiles" }
            }
        };

        // Act
        var errors = CommandValidator.ValidateCommandLine(command);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateCommandLine_CommandWithNoArguments_ShouldReturnNoErrors()
    {
        // Arrange
        var command = new CommandLine 
        { 
            Command = "/cleartarget",
            Arguments = new List<CommandArgument>()
        };

        // Act
        var errors = CommandValidator.ValidateCommandLine(command);

        // Assert
        Assert.Empty(errors);
    }
} 