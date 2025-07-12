using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.Core.Services;

namespace EasyWoWMacro.Tests;

public class ValidationEnhancementServiceTests
{
    private readonly ValidationEnhancementService _enhancer = new();
    private readonly MacroParser _parser = new();

    [Fact]
    public void EnhanceValidationErrors_WithIncompleteClause_ShouldDetectError()
    {
        // Arrange
        var macroText = "/cast [@target,harm,nodead] Smite; [@target,help,nodead]";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var incompleteClauseError = enhancedErrors.FirstOrDefault(e => 
            e.Type == ValidationErrorType.StructureError && 
            e.Message.Contains("Incomplete clause detected"));
        
        Assert.NotNull(incompleteClauseError);
        Assert.Contains("conditional without action", incompleteClauseError.Message);
    }

    [Fact]
    public void EnhanceValidationErrors_WithMultipleCommandLines_ShouldDetectBothErrors()
    {
        // Arrange - This is the exact macro the user mentioned
        var macroText = @"/cast [@target,harm,nodead] Smite; [@target,help,nodead]
/use [combat] Healthstone
/say [combat] Using Healthstone!";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        Assert.True(enhancedErrors.Count >= 2, $"Expected at least 2 errors, got {enhancedErrors.Count}");
        
        // Should detect incomplete clause
        var incompleteClauseError = enhancedErrors.FirstOrDefault(e => 
            e.Type == ValidationErrorType.StructureError && 
            e.Message.Contains("Incomplete clause detected"));
        Assert.NotNull(incompleteClauseError);

        // Should detect multiple command lines
        var multiLineError = enhancedErrors.FirstOrDefault(e => 
            e.Type == ValidationErrorType.StructureError && 
            e.Message.Contains("Multiple command lines detected"));
        Assert.NotNull(multiLineError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithArgumentsBeforeConditionals_ShouldDetectError()
    {
        // Arrange
        var macroText = "/cast Fireball [harm]";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var structureError = enhancedErrors.FirstOrDefault(e => 
            e.Type == ValidationErrorType.StructureError && 
            e.Message.Contains("Arguments appear before conditionals"));
        
        Assert.NotNull(structureError);
        Assert.Contains("conditionals must come immediately after the command", structureError.Explanation);
    }

    [Fact]
    public void EnhanceValidationErrors_WithValidMacroNoShowtooltip_ShouldSuggestShowtooltip()
    {
        // Arrange
        var macroText = "/cast [help] Heal; [harm] Fireball";
        var basicErrors = new List<string>(); // No basic errors

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var suggestionError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("#showtooltip"));
        
        Assert.NotNull(suggestionError);
        Assert.Contains("💡 Suggestion", suggestionError.Message);
        Assert.Equal(ValidationErrorType.GeneralError, suggestionError.Type);
    }

    [Fact]
    public void EnhanceValidationErrors_WithBasicErrors_ShouldNotSuggestShowtooltip()
    {
        // Arrange
        var macroText = "/cast [help] Heal; [harm] Fireball";
        var basicErrors = new List<string> { "Some validation error" };

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var suggestionError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("#showtooltip"));
        
        Assert.Null(suggestionError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithCharacterLimitError_ShouldEnhanceCorrectly()
    {
        // Arrange
        var basicErrors = new List<string> { "Macro exceeds WoW's 255 character limit (300 characters)." };
        var macroText = "some long macro text";

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var characterError = enhancedErrors.First();
        Assert.Equal(ValidationErrorType.CharacterLimit, characterError.Type);
        Assert.Contains("255 character limit", characterError.Explanation);
        Assert.NotNull(characterError.QuickFix);
    }

    [Fact]
    public void EnhanceValidationErrors_WithBracketError_ShouldEnhanceCorrectly()
    {
        // Arrange
        var basicErrors = new List<string> { "Unmatched bracket detected" };
        var macroText = "/cast [help Heal";

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var bracketError = enhancedErrors.First();
        Assert.Equal(ValidationErrorType.SyntaxError, bracketError.Type);
        Assert.Contains("properly opened and closed", bracketError.Explanation);
        Assert.Contains("Correct: /cast [help] Heal", bracketError.Example);
    }

    [Fact]
    public void EnhanceValidationErrors_WithInvalidCommand_ShouldEnhanceCorrectly()
    {
        // Arrange
        var basicErrors = new List<string> { "Invalid command: /invalidcommand" };
        var macroText = "/invalidcommand something";

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var commandError = enhancedErrors.First();
        Assert.Equal(ValidationErrorType.CommandError, commandError.Type);
        Assert.Contains("not recognized as a valid WoW macro command", commandError.Explanation);
        Assert.Equal("commands", commandError.GuideSection);
    }

    [Fact]
    public void EnhanceValidationErrors_WithMultipleGcdAbilities_ShouldDetectError()
    {
        // Arrange
        var macroText = "/cast Fireball; /cast Frostbolt";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var gcdError = enhancedErrors.FirstOrDefault(e => 
            e.Type == ValidationErrorType.StructureError && 
            e.Message.Contains("Multiple abilities that trigger Global Cooldown"));
        
        Assert.NotNull(gcdError);
        Assert.Contains("Only one ability that triggers the Global Cooldown", gcdError.Explanation);
    }
}