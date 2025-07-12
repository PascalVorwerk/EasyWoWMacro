using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.Core.Services;

namespace EasyWoWMacro.Tests;

public class ValidationEnhancementServiceEdgeCaseTests
{
    private readonly ValidationEnhancementService _enhancer = new();
    private readonly MacroParser _parser = new();

    [Fact]
    public void EnhanceValidationErrors_WithTrailingSemicolon_ShouldDetectUselessSyntax()
    {
        // Arrange - Macro with trailing semicolon that creates empty clause
        var macroText = "/cast Fireball;";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var trailingSemicolonError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("trailing semicolon") || 
            e.Message.Contains("empty clause"));
        
        Assert.NotNull(trailingSemicolonError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithNestedConditionals_ShouldDetectInvalidSyntax()
    {
        // Arrange - Invalid nested conditionals
        var macroText = "/cast [mod:shift,[harm]] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var nestedError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("nested") || 
            e.Type == ValidationErrorType.SyntaxError);
        
        Assert.NotNull(nestedError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithSpacesInConditionals_ShouldSuggestCleanup()
    {
        // Arrange - Conditionals with unnecessary spaces
        var macroText = "/cast [ mod:shift , harm ] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var spacingError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("spaces") && 
            e.Type == ValidationErrorType.StructureError);
        
        Assert.NotNull(spacingError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithMissingForwardSlash_ShouldDetectInvalidCommand()
    {
        // Arrange - Command without leading slash
        var macroText = "cast Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var missingSlashError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("missing") && 
            e.Message.Contains("/"));
        
        Assert.NotNull(missingSlashError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithMultipleCommandsOnSameLine_ShouldDetectError()
    {
        // Arrange - Multiple commands separated by spaces (not semicolons)
        var macroText = "/cast Fireball /use Potion";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var multipleCommandsError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("Multiple commands") && 
            e.Message.Contains("same line"));
        
        Assert.NotNull(multipleCommandsError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithInvalidTargetSyntax_ShouldDetectError()
    {
        // Arrange - Invalid target syntax
        var macroText = "/cast [@] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var targetError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("target") && 
            e.Type == ValidationErrorType.ConditionalError);
        
        Assert.NotNull(targetError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithDirectiveInWrongPlace_ShouldDetectError()
    {
        // Arrange - #showtooltip not at the beginning
        var macroText = @"/cast Fireball
#showtooltip Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var directiveOrderError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("Directive") && 
            e.Message.Contains("beginning"));
        
        Assert.NotNull(directiveOrderError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithTypoInConditional_ShouldSuggestCorrection()
    {
        // Arrange - Common typo in conditional
        var macroText = "/cast [mod:shfit] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var typoError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("shift") || 
            e.Message.Contains("Did you mean"));
        
        Assert.NotNull(typoError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithNearCharacterLimit_ShouldWarnBeforeLimit()
    {
        // Arrange - Macro close to character limit (240+ characters)
        var macroText = "/cast [mod:shift,combat,harm,nodead,exists] Fireball; [mod:ctrl,combat,harm,nodead,exists] Frostbolt; [mod:alt,combat,harm,nodead,exists] Polymorph; [combat,harm,nodead,exists] Fireball; [@target,harm,nodead] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var nearLimitWarning = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("Approaching") && 
            e.Message.Contains("character limit"));
        
        Assert.NotNull(nearLimitWarning);
    }

    [Fact]
    public void EnhanceValidationErrors_WithOnlyDirectives_ShouldSuggestAddingCommands()
    {
        // Arrange - Macro with only directives
        var macroText = @"#showtooltip Fireball
#show Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var noCommandsError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("commands") && 
            e.Message.Contains("directive"));
        
        Assert.NotNull(noCommandsError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithExcessiveWhitespace_ShouldSuggestCleanup()
    {
        // Arrange - Macro with excessive whitespace
        var macroText = "/cast    Fireball  ;    [mod:shift]   Frostbolt";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var whitespaceError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("whitespace") || 
            e.Message.Contains("spaces"));
        
        Assert.NotNull(whitespaceError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithInvalidItemSyntax_ShouldDetectError()
    {
        // Arrange - Invalid item syntax
        var macroText = "/use item Healthstone";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var itemSyntaxError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("item syntax") && 
            e.QuickFix?.Contains("item:") == true);
        
        Assert.NotNull(itemSyntaxError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithConditionalTypos_ShouldSuggestCommonFixes()
    {
        // Arrange - Multiple common typos
        var macroText = "/cast [modifier:shift,target=focus,combat] Fireball";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var typoSuggestions = enhancedErrors.Where(e => 
            e.Message.Contains("Did you mean") || 
            e.QuickFix?.Contains("mod:") == true ||
            e.QuickFix?.Contains("@focus") == true).ToList();
        
        Assert.NotEmpty(typoSuggestions);
    }

    [Fact]
    public void EnhanceValidationErrors_WithMixedCaseCommands_ShouldNormalizeCase()
    {
        // Arrange - Mixed case commands
        var macroText = "/CAST fireball; /Use potion";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var caseError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("case") && 
            e.QuickFix?.Contains("lowercase") == true);
        
        Assert.NotNull(caseError);
    }

    [Fact]
    public void EnhanceValidationErrors_WithRedundantConditionals_ShouldDetectOptimization()
    {
        // Arrange - Redundant conditionals that could be simplified
        var macroText = "/cast [combat,combat] Fireball; [mod:shift,mod:shift] Frostbolt";
        var basicErrors = _parser.ValidateMacroText(macroText);

        // Act
        var enhancedErrors = _enhancer.EnhanceValidationErrors(basicErrors, macroText);

        // Assert
        var redundantError = enhancedErrors.FirstOrDefault(e => 
            e.Message.Contains("redundant") || 
            e.Message.Contains("duplicate"));
        
        Assert.NotNull(redundantError);
    }
}