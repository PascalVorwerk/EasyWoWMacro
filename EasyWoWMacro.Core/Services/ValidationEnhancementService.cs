using EasyWoWMacro.Core.Models;
using System.Text.RegularExpressions;

namespace EasyWoWMacro.Core.Services;

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

        // Check for trailing semicolons
        if (HasTrailingSemicolon(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Trailing semicolon creates empty clause",
                Explanation = "Semicolons at the end of lines create empty clauses that serve no purpose and can cause confusion.",
                QuickFix = "Remove trailing semicolons",
                Example = "Wrong: /cast Fireball;  →  Correct: /cast Fireball",
                GuideSection = "structure"
            });
        }

        // Check for excessive whitespace
        if (HasExcessiveWhitespace(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Excessive whitespace detected",
                Explanation = "Multiple consecutive spaces waste character count and can make macros harder to read.",
                QuickFix = "Clean up extra spaces",
                Example = "Wrong: /cast    Fireball  →  Correct: /cast Fireball",
                GuideSection = "structure"
            });
        }

        // Check for spaces in conditionals
        if (HasSpacesInConditionals(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Unnecessary spaces in conditionals",
                Explanation = "Spaces inside conditional brackets are not needed and waste character count.",
                QuickFix = "Remove spaces from conditionals",
                Example = "Wrong: [ mod:shift ]  →  Correct: [mod:shift]",
                GuideSection = "conditionals"
            });
        }

        // Check for missing forward slash
        if (HasMissingForwardSlash(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.CommandError,
                Message = "Command missing / forward slash",
                Explanation = "All WoW macro commands must start with a forward slash (/).",
                QuickFix = "Add / before the command",
                Example = "Wrong: cast Fireball  →  Correct: /cast Fireball",
                GuideSection = "commands"
            });
        }

        // Check for multiple commands on same line
        if (HasMultipleCommandsOnSameLine(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Multiple commands on same line without proper separation",
                Explanation = "Commands should be separated by semicolons, not spaces.",
                QuickFix = "Use semicolons to separate commands",
                Example = "Wrong: /cast Fireball /use Potion  →  Correct: /cast Fireball; /use Potion",
                GuideSection = "structure"
            });
        }

        // Check for invalid target syntax
        if (HasInvalidTargetSyntax(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.ConditionalError,
                Message = "Invalid target syntax detected",
                Explanation = "Target conditionals must specify a valid target after the @ symbol.",
                QuickFix = "Specify a target after @",
                Example = "Wrong: [@]  →  Correct: [@target] or [@focus]",
                GuideSection = "conditionals"
            });
        }

        // Check for directive placement
        if (HasDirectiveInWrongPlace(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Directive should be at the beginning of macro",
                Explanation = "Directives like #showtooltip should be placed at the start of the macro for proper functionality.",
                QuickFix = "Move directives to the beginning",
                Example = "Place #showtooltip at the top of your macro",
                GuideSection = "commands"
            });
        }

        // Check for near character limit
        if (IsNearCharacterLimit(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.CharacterLimit,
                Message = "Approaching character limit - consider optimization",
                Explanation = "Your macro is close to the 255 character limit. Consider shortening spell names or removing unnecessary elements.",
                QuickFix = "Optimize macro length",
                Example = "Use shorter spell names or remove extra spaces",
                GuideSection = "basics"
            });
        }

        // Check for only directives
        if (HasOnlyDirectives(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Macro contains only directives without commands",
                Explanation = "A macro with only directives won't perform any actions. Add at least one command.",
                QuickFix = "Add commands to your macro",
                Example = "Add commands like /cast, /use, or /say after your directives",
                GuideSection = "commands"
            });
        }

        // Check for invalid item syntax
        if (HasInvalidItemSyntax(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.CommandError,
                Message = "Invalid item syntax detected",
                Explanation = "Item references should use the format 'item:ID' or be spelled out exactly.",
                QuickFix = "Use item: syntax for items",
                Example = "Use: /use item:12345 instead of /use item Healthstone",
                GuideSection = "commands"
            });
        }

        // Check for nested conditionals
        if (HasNestedConditionals(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.SyntaxError,
                Message = "Nested conditionals are not supported",
                Explanation = "WoW macros do not support nested conditional brackets. Use separate conditions instead.",
                QuickFix = "Remove nested brackets",
                Example = "Wrong: [mod:shift,[harm]]  →  Correct: [mod:shift,harm]",
                GuideSection = "conditionals"
            });
        }

        // Check for common conditional typos
        CheckConditionalTypos(macroText, issues);

        // Check for mixed case commands
        if (HasMixedCaseCommands(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.CommandError,
                Message = "Mixed case commands detected",
                Explanation = "While WoW accepts mixed case, lowercase commands are standard and more readable.",
                QuickFix = "Use lowercase commands",
                Example = "Use: /cast instead of /CAST",
                GuideSection = "commands"
            });
        }

        // Check for redundant conditionals
        if (HasRedundantConditionals(macroText))
        {
            issues.Add(new ValidationError
            {
                Type = ValidationErrorType.StructureError,
                Message = "Redundant or duplicate conditionals detected",
                Explanation = "Duplicate conditionals within the same bracket set are redundant and waste character count.",
                QuickFix = "Remove duplicate conditionals",
                Example = "Wrong: [combat,combat]  →  Correct: [combat]",
                GuideSection = "conditionals"
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

    private static bool HasTrailingSemicolon(string macroText)
    {
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines.Any(line => line.TrimEnd().EndsWith(';'));
    }

    private static bool HasExcessiveWhitespace(string macroText)
    {
        return macroText.Contains("  ") || macroText.Contains("\t");
    }

    private static bool HasSpacesInConditionals(string macroText)
    {
        var pattern = @"\[\s+[^\]]*\s+\]";
        return Regex.IsMatch(macroText, pattern);
    }

    private static bool HasMissingForwardSlash(string macroText)
    {
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith('#') && !string.IsNullOrEmpty(trimmed))
            {
                // Check if it looks like a command but doesn't start with /
                if (Regex.IsMatch(trimmed, @"^(cast|use|say|yell|tell|party|guild|raid|whisper|emote)\s+", RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool HasMultipleCommandsOnSameLine(string macroText)
    {
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            // Count commands separated by spaces (not semicolons)
            var parts = line.Split(' ');
            var commandCount = 0;
            foreach (var part in parts)
            {
                if (part.StartsWith('/'))
                {
                    commandCount++;
                }
            }
            if (commandCount > 1)
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasInvalidTargetSyntax(string macroText)
    {
        // Check for [@] without target specification
        return macroText.Contains("[@]") || Regex.IsMatch(macroText, @"\[@\s*\]");
    }

    private static bool HasDirectiveInWrongPlace(string macroText)
    {
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        bool foundCommand = false;
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith('/'))
            {
                foundCommand = true;
            }
            else if (foundCommand && trimmed.StartsWith('#'))
            {
                return true; // Directive after command
            }
        }
        return false;
    }

    private static bool IsNearCharacterLimit(string macroText)
    {
        return macroText.Length >= 215; // Warn at ~85% of 255 character limit
    }

    private static bool HasOnlyDirectives(string macroText)
    {
        var lines = macroText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines.All(line => line.Trim().StartsWith('#') || string.IsNullOrWhiteSpace(line));
    }

    private static void CheckConditionalTypos(string macroText, List<ValidationError> issues)
    {
        var commonTypos = new Dictionary<string, string>
        {
            { "modifier:", "mod:" },
            { "target=", "@" },
            { "shfit", "shift" },
            { "shfit:", "shift:" },
            { "controler:", "ctrl:" },
            { "alternative:", "alt:" },
            { "button:", "btn:" }
        };

        foreach (var typo in commonTypos)
        {
            if (macroText.Contains(typo.Key))
            {
                issues.Add(new ValidationError
                {
                    Type = ValidationErrorType.ConditionalError,
                    Message = $"Did you mean '{typo.Value}'?",
                    Explanation = $"'{typo.Key}' should be '{typo.Value}' in WoW macro conditionals.",
                    QuickFix = $"Replace '{typo.Key}' with '{typo.Value}'",
                    Example = $"Use: {typo.Value} instead of {typo.Key}",
                    GuideSection = "conditionals"
                });
            }
        }
    }

    private static bool HasMixedCaseCommands(string macroText)
    {
        var pattern = @"/(CAST|USE|SAY|YELL|TELL|PARTY|GUILD|RAID|WHISPER|EMOTE)\s";
        return Regex.IsMatch(macroText, pattern);
    }

    private static bool HasRedundantConditionals(string macroText)
    {
        var pattern = @"\[([^\]]*)\]";
        var matches = Regex.Matches(macroText, pattern);
        
        foreach (Match match in matches)
        {
            var conditions = match.Groups[1].Value.Split(',');
            var seenConditions = new HashSet<string>();
            
            foreach (var condition in conditions)
            {
                var trimmed = condition.Trim();
                if (!seenConditions.Add(trimmed) && !string.IsNullOrEmpty(trimmed))
                {
                    return true; // Duplicate found
                }
            }
        }
        return false;
    }

    private static bool HasInvalidItemSyntax(string macroText)
    {
        // Check for patterns like "/use item Something" instead of "/use item:ID"
        return Regex.IsMatch(macroText, @"/use\s+item\s+[A-Za-z]", RegexOptions.IgnoreCase);
    }

    private static bool HasNestedConditionals(string macroText)
    {
        // Check for patterns like [condition,[nested]] or [condition[nested]]
        return Regex.IsMatch(macroText, @"\[[^\]]*\[[^\]]*\]");
    }
}