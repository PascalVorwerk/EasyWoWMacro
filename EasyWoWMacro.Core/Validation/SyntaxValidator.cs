using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing.Utilities;
using System.Text.RegularExpressions;

namespace EasyWoWMacro.Core.Validation;

/// <summary>
/// Validates WoW macro syntax
/// </summary>
public static class SyntaxValidator
{
    /// <summary>
    /// Validates macro text and returns any syntax errors
    /// </summary>
    /// <param name="macroText">The macro text to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateMacroText(string macroText)
    {
        var errors = new List<string>();
        var lines = macroText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length == 0)
        {
            errors.Add("Macro text is empty");
            return errors;
        }
        
        var hasCommands = false;
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;
                
            // Check for valid directives
            if (trimmedLine.StartsWith("#"))
            {
                if (!IsValidDirective(trimmedLine))
                {
                    errors.Add($"Invalid directive: {trimmedLine}");
                }
                continue;
            }
            
            // Check for valid slash commands
            if (trimmedLine.StartsWith("/"))
            {
                if (!IsValidSlashCommand(trimmedLine))
                {
                    errors.Add($"Invalid slash command: {trimmedLine}");
                }
                
                // Check for unclosed brackets in conditionals
                var bracketErrors = ValidateBrackets(trimmedLine);
                errors.AddRange(bracketErrors);
                
                hasCommands = true;
            }
            else
            {
                // Non-slash commands are also valid (like spell names)
                hasCommands = true;
            }
        }
        
        if (!hasCommands)
        {
            errors.Add("Macro must contain at least one command");
        }
        
        return errors;
    }

    /// <summary>
    /// Validates bracket syntax in a command line
    /// </summary>
    /// <param name="line">The line to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateBrackets(string line)
    {
        var errors = new List<string>();
        var bracketCount = 0;
        var bracketStart = -1;
        var inBracket = false;
        var bracketContent = "";
        
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '[')
            {
                bracketCount++;
                if (bracketCount == 1)
                {
                    bracketStart = i;
                    inBracket = true;
                    bracketContent = "";
                }
                else if (inBracket)
                {
                    // Nested brackets are not allowed in WoW macros
                    errors.Add($"Nested brackets are not allowed at position {i + 1}");
                    return errors;
                }
            }
            else if (line[i] == ']')
            {
                bracketCount--;
                if (bracketCount < 0)
                {
                    errors.Add($"Unexpected closing bracket ']' at position {i + 1}");
                    bracketCount = 0;
                }
                else if (bracketCount == 0)
                {
                    inBracket = false;
                    // Validate the content inside the bracket
                    var contentErrors = ValidateBracketContent(bracketContent, bracketStart + 1);
                    errors.AddRange(contentErrors);
                }
            }
            else if (inBracket)
            {
                bracketContent += line[i];
            }
        }
        
        if (bracketCount > 0)
        {
            errors.Add($"Unclosed conditional bracket '[' starting at position {bracketStart + 1}");
        }
        
        // Add validation for malformed semicolon-separated clauses
        var semicolonErrors = ValidateSemicolonSeparatedClauses(line);
        errors.AddRange(semicolonErrors);

        // Check for non-bracketed conditional lists after a bracketed clause
        var commandMatch = Regex.Match(line, @"^(/\w+)");
        if (commandMatch.Success)
        {
            var rest = line.Substring(commandMatch.Length).Trim();
            var tokens = StringUtilities.TokenizeCommandLine(rest);
            
            // Scan for bracketed clause followed by non-bracketed conditional list
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].StartsWith("[") && !tokens[i+1].StartsWith("[") && 
                    (tokens[i+1].StartsWith("@") || StringUtilities.StartsWithKnownConditional(tokens[i+1])))
                {
                    // If the next token contains a comma, it's likely a conditional list
                    if (tokens[i+1].Contains(','))
                    {
                        errors.Add($"Malformed clause: '{tokens[i+1]}' - conditionals must be enclosed in brackets []");
                    }
                }
            }
        }
        
        return errors;
    }

    /// <summary>
    /// Validates the content inside a bracket
    /// </summary>
    /// <param name="content">The bracket content</param>
    /// <param name="startPosition">The position where the bracket starts</param>
    /// <returns>List of validation errors</returns>
    private static List<string> ValidateBracketContent(string content, int startPosition)
    {
        var errors = new List<string>();
        
        // Empty conditionals are valid in WoW macros - they mean "always execute this action"
        if (string.IsNullOrWhiteSpace(content))
        {
            return errors; // No error for empty conditionals
        }
        
        // Check for basic conditional syntax
        var conditions = content.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var condition in conditions)
        {
            var trimmedCondition = condition.Trim();
            if (string.IsNullOrWhiteSpace(trimmedCondition))
            {
                errors.Add($"Empty condition in bracket at position {startPosition}");
                continue;
            }
            
            // Check if it's a valid conditional format
            if (!IsValidConditionalFormat(trimmedCondition))
            {
                errors.Add($"Invalid conditional format '{trimmedCondition}' in bracket at position {startPosition}");
            }
        }
        
        return errors;
    }

    /// <summary>
    /// Validates semicolon-separated clauses
    /// </summary>
    /// <param name="line">The line to validate</param>
    /// <returns>List of validation errors</returns>
    private static List<string> ValidateSemicolonSeparatedClauses(string line)
    {
        var errors = new List<string>();
        
        // Split by semicolons to check each clause
        var semicolonParts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in semicolonParts)
        {
            var trimmed = part.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            // If the clause starts with a conditional list and is not bracketed, this is invalid
            if (!trimmed.StartsWith("[") && (trimmed.StartsWith("@") || StringUtilities.StartsWithKnownConditional(trimmed)))
            {
                // If the first word contains a comma, it's likely a conditional list
                var firstWord = trimmed.Split(' ', 2)[0];
                if (firstWord.Contains(','))
                {
                    errors.Add($"Malformed clause: '{trimmed}' - conditionals must be enclosed in brackets []");
                }
            }
        }
        
        return errors;
    }

    /// <summary>
    /// Checks if a directive is valid
    /// </summary>
    /// <param name="directive">The directive to check</param>
    /// <returns>True if valid</returns>
    private static bool IsValidDirective(string directive)
    {
        return WoWMacroConstants.ValidDirectives.Any(valid => 
            directive.StartsWith(valid, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if a slash command is valid
    /// </summary>
    /// <param name="command">The command to check</param>
    /// <returns>True if valid</returns>
    private static bool IsValidSlashCommand(string command)
    {
        return WoWMacroConstants.ValidSlashCommands.Any(valid => 
            command.StartsWith(valid, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if a conditional format is valid
    /// </summary>
    /// <param name="condition">The condition to check</param>
    /// <returns>True if valid</returns>
    private static bool IsValidConditionalFormat(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return false;
            
        // Parse the condition into key and value
        var colonIndex = condition.IndexOf(':');
        string key;
        string? value = null;
        
        if (colonIndex > 0)
        {
            key = condition.Substring(0, colonIndex).Trim();
            value = condition.Substring(colonIndex + 1).Trim();
            
            // Both key and value should not be empty
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return false;
        }
        else
        {
            key = condition.Trim();
        }
        
        // Use ConditionalValidator as the single source of truth
        var conditionObj = new Condition { Key = key, Value = value };
        return ConditionalValidator.IsValidCondition(conditionObj);
    }
} 