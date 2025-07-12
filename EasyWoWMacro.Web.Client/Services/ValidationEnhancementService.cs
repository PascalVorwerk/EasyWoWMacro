namespace EasyWoWMacro.Web.Client.Services;

public class ValidationEnhancementService
{
    public List<ValidationError> EnhanceValidationErrors(List<string> basicErrors, string macroText)
    {
        var enhancedErrors = new List<ValidationError>();

        foreach (var error in basicErrors)
        {
            var enhancedError = CreateEnhancedError(error, macroText);
            enhancedErrors.Add(enhancedError);
        }

        // Add contextual analysis for common patterns
        enhancedErrors.AddRange(AnalyzeMacroForCommonIssues(macroText, basicErrors));

        return enhancedErrors;
    }

    private ValidationError CreateEnhancedError(string basicError, string macroText)
    {
        var error = new ValidationError { Message = basicError };

        // Character limit error
        if (basicError.Contains("255 character limit"))
        {
            error.Type = ValidationErrorType.CharacterLimit;
            error.Explanation = "WoW macros have a strict 255 character limit. Consider shortening spell names, removing unnecessary spaces, or splitting into multiple macros.";
            error.QuickFix = "Try using shorter spell names or removing extra spaces";
            error.GuideSection = "basics";
            return error;
        }

        // Bracket matching errors
        if (basicError.Contains("bracket") || basicError.Contains("Unmatched"))
        {
            error.Type = ValidationErrorType.SyntaxError;
            error.Explanation = "All conditional brackets must be properly opened and closed. Each '[' needs a matching ']'.";
            error.QuickFix = "Check that every '[' has a matching ']'";
            error.Example = "Correct: /cast [help] Heal";
            error.GuideSection = "conditionals";
            return error;
        }

        // Invalid command errors
        if (basicError.Contains("Invalid command") || basicError.Contains("Unknown command"))
        {
            error.Type = ValidationErrorType.CommandError;
            error.Explanation = "The command is not recognized as a valid WoW macro command. Check spelling and ensure you're using the correct command syntax.";
            error.QuickFix = "Verify the command spelling and check our command reference";
            error.GuideSection = "commands";
            return error;
        }

        // Conditional errors
        if (basicError.Contains("conditional") || basicError.Contains("condition"))
        {
            error.Type = ValidationErrorType.ConditionalError;
            error.Explanation = "The conditional syntax is incorrect. Conditionals should be enclosed in brackets and use valid condition names.";
            error.QuickFix = "Check conditional spelling and syntax";
            error.Example = "Correct: [help], [harm], [mod:shift]";
            error.GuideSection = "conditionals";
            return error;
        }

        // Default to general error
        error.Type = ValidationErrorType.GeneralError;
        error.GuideSection = "structure";
        return error;
    }

    private List<ValidationError> AnalyzeMacroForCommonIssues(string macroText, List<string> basicErrors)
    {
        var issues = new List<ValidationError>();

        // Check for arguments before conditionals (common mistake)
        if (HasArgumentsBeforeConditionals(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Arguments appear before conditionals",
                Explanation = "In WoW macros, conditionals must come immediately after the command, before any arguments.",
                QuickFix = "Move conditionals before arguments",
                Example = "Wrong: /cast Fireball [harm]  →  Correct: /cast [harm] Fireball",
                GuideSection = "structure"
            });
        }

        // Check for missing #showtooltip (helpful suggestion, only if no other errors)
        if (basicErrors.Count == 0 && !macroText.Contains("#showtooltip") && !macroText.Contains("#show"))
        {
            var castCommands = Regex.Matches(macroText, @"/cast\s+", RegexOptions.IgnoreCase);
            if (castCommands.Count > 0)
            {
                issues.Add(new ValidationError
                {
                    Type = ValidationErrorType.GeneralError,
                    Message = "💡 Suggestion: Consider adding #showtooltip for better usability",
                    Explanation = "#showtooltip makes your macro button show the appropriate spell icon and tooltip information. This is optional but recommended.",
                    QuickFix = "Add '#showtooltip' as the first line (optional)",
                    Example = "#showtooltip\n/cast [help] Heal; [harm] Fireball",
                    GuideSection = "commands"
                });
            }
        }

        // Check for multiple GCD abilities (common mistake)
        if (HasMultipleGcdAbilities(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Multiple abilities that trigger Global Cooldown detected",
                Explanation = "Only one ability that triggers the Global Cooldown (GCD) will execute per macro activation. Consider using conditionals instead.",
                QuickFix = "Use conditionals to choose between abilities",
                Example = "Instead of: /cast Fireball; /cast Heal  →  Use: /cast [harm] Fireball; [help] Heal",
                GuideSection = "conditionals"
            });
        }

        // Check for incomplete clauses (conditionals without actions)
        if (HasIncompleteClause(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Incomplete clause detected - conditional without action",
                Explanation = "A conditional clause was found without a command or spell to execute. Each conditional must be followed by an action.",
                QuickFix = "Add a command or spell after the conditional",
                Example = "Wrong: /cast Fireball; [@arena2,actionbar:1]  →  Correct: /cast Fireball; [@arena2,actionbar:1] Polymorph",
                GuideSection = "structure"
            });
        }

        // Check for potential multi-line execution issues
        var allLines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var commandLineCount = 0;

        foreach (var line in allLines)
        {
            if (!string.IsNullOrWhiteSpace(line) && line.Trim().StartsWith("/"))
            {
                var trimmedLine = line.TrimStart();
                if (trimmedLine.StartsWith("/cast", StringComparison.OrdinalIgnoreCase) ||
                    trimmedLine.StartsWith("/use", StringComparison.OrdinalIgnoreCase))
                {
                    commandLineCount++;
                }
            }
        }

        if (commandLineCount > 1)
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Multiple command lines detected - execution behavior may be unexpected",
                Explanation = "WoW macros execute all lines simultaneously, but only one GCD ability will trigger per button press. Consider using conditionals on a single line instead.",
                QuickFix = "Combine commands into a single line with conditionals",
                Example = "Instead of:\n/cast [harm] Smite\n/use [combat] Healthstone\n\nConsider:\n/cast [harm] Smite; [combat] Healthstone",
                GuideSection = "structure"
            });
        }

        return issues;
    }

    private bool HasArgumentsBeforeConditionals(string macroText)
    {
        // Look for pattern like "/cast SpellName [condition]"
        var pattern = @"/\w+\s+[A-Za-z][^[\]]*\[[^\]]+\]";
        return Regex.IsMatch(macroText, pattern);
    }

    private bool HasMultipleGcdAbilities(string macroText)
    {
        // Count /cast commands that aren't separated by conditionals
        var lines = macroText.Split('\n');
        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("/cast", StringComparison.OrdinalIgnoreCase))
            {
                // Count semicolon-separated cast commands without conditionals
                var parts = line.Split(';');
                var castCount = 0;
                foreach (var part in parts)
                {
                    if (part.Contains("/cast") && !part.Contains('['))
                    {
                        castCount++;
                    }
                }
                if (castCount > 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool HasIncompleteClause(string macroText)
    {
        // Look for clauses that are just conditionals without any command/spell after them
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("/"))
            {
                // Split by semicolons to check each clause
                var clauses = line.Split(';');

                foreach (var clause in clauses)
                {
                    var trimmedClause = clause.Trim();

                    // Skip empty clauses
                    if (string.IsNullOrWhiteSpace(trimmedClause))
                        continue;

                    // Check if clause starts with a conditional but has no command
                    if (trimmedClause.StartsWith('[') && trimmedClause.Contains(']'))
                    {
                        var endBracket = trimmedClause.IndexOf(']');
                        var afterConditional = trimmedClause.Substring(endBracket + 1).Trim();

                        // If nothing meaningful after the conditional, it's incomplete
                        if (string.IsNullOrWhiteSpace(afterConditional))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}