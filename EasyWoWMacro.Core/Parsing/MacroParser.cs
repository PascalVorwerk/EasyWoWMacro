using EasyWoWMacro.Core.Models;
using System.Text.RegularExpressions;

namespace EasyWoWMacro.Core.Parsing;

/// <summary>
/// Parser for World of Warcraft macros
/// </summary>
public class MacroParser
{
    /// <summary>
    /// Parses macro text and creates a Macro object
    /// </summary>
    /// <param name="macroText">The macro text to parse</param>
    /// <returns>A Macro object representing the parsed macro</returns>
    public Macro Parse(string macroText)
    {
        var macro = new Macro();
        var lines = macroText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int lineNumber = 1;
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            MacroLine macroLine;
            if (trimmedLine.StartsWith("#"))
            {
                // Directive
                var parts = trimmedLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                macroLine = new DirectiveLine
                {
                    LineNumber = lineNumber,
                    RawText = trimmedLine,
                    Directive = parts[0],
                    Argument = parts.Length > 1 ? parts[1] : null
                };
            }
            else if (trimmedLine.StartsWith("/"))
            {
                // Command with conditionals and arguments
                macroLine = ParseCommandLine(trimmedLine, lineNumber);
            }
            else if (trimmedLine.StartsWith(";"))
            {
                // Comment
                macroLine = new CommentLine
                {
                    LineNumber = lineNumber,
                    RawText = trimmedLine,
                    Comment = trimmedLine.Substring(1).Trim()
                };
            }
            else
            {
                // Fallback: treat as argument to previous command or as a comment
                macroLine = new CommentLine
                {
                    LineNumber = lineNumber,
                    RawText = trimmedLine,
                    Comment = trimmedLine
                };
            }
            macro.Lines.Add(macroLine);
            lineNumber++;
        }
        return macro;
    }

    private CommandLine ParseCommandLine(string line, int lineNumber)
    {
        // Extract command name (e.g., /cast, /use)
        var commandMatch = Regex.Match(line, @"^(/\w+)");
        var command = commandMatch.Groups[1].Value;
        var rest = line.Substring(command.Length).Trim();

        var commandLine = new CommandLine
        {
            LineNumber = lineNumber,
            RawText = line,
            Command = command,
            Arguments = new List<CommandArgument>(),
            Conditionals = null
        };

        // Parse conditionals and arguments
        if (!string.IsNullOrWhiteSpace(rest))
        {
            var (conditionals, arguments) = ParseConditionalsAndArguments(rest);
            commandLine.Conditionals = conditionals;
            commandLine.Arguments = arguments;
        }

        return commandLine;
    }

    private List<string> SplitBySemicolonsOutsideBrackets(string input)
    {
        var result = new List<string>();
        int bracketDepth = 0;
        int lastSplit = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '[') bracketDepth++;
            else if (input[i] == ']') bracketDepth--;
            else if (input[i] == ';' && bracketDepth == 0)
            {
                result.Add(input.Substring(lastSplit, i - lastSplit));
                lastSplit = i + 1;
            }
        }
        if (lastSplit < input.Length)
            result.Add(input.Substring(lastSplit));
        return result;
    }

    private (Conditional? conditionals, List<CommandArgument> arguments) ParseConditionalsAndArguments(string rest)
    {
        var conditionals = new Conditional();
        var arguments = new List<CommandArgument>();

        // Use the new helper to split only on semicolons outside brackets
        var semicolonParts = SplitBySemicolonsOutsideBrackets(rest);
        foreach (var part in semicolonParts)
        {
            var trimmedPart = part.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedPart))
            {
                var (partConditionals, partArguments) = ParseSingleConditionalArgumentPair(trimmedPart);
                if (partConditionals != null)
                {
                    conditionals.ConditionSets.AddRange(partConditionals.ConditionSets);
                }
                arguments.AddRange(partArguments);
            }
        }

        return conditionals.ConditionSets.Count > 0 ? (conditionals, arguments) : (null, arguments);
    }

    private (Conditional? conditionals, List<CommandArgument> arguments) ParseSingleConditionalArgumentPair(string text)
    {
        var conditionals = new Conditional();
        var arguments = new List<CommandArgument>();
        var currentPosition = 0;

        // Parse conditionals first (anything in brackets)
        while (currentPosition < text.Length)
        {
            var bracketStart = text.IndexOf('[', currentPosition);
            if (bracketStart == -1)
                break;

            var bracketEnd = FindMatchingBracket(text, bracketStart);
            if (bracketEnd == -1)
            {
                // Invalid bracket structure - treat the rest as argument
                var invalidBracketText = text.Substring(currentPosition).Trim();
                if (!string.IsNullOrWhiteSpace(invalidBracketText))
                {
                    arguments.Add(new CommandArgument { Value = invalidBracketText });
                }
                break;
            }

            var conditionalText = text.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
            var conditionSets = ParseConditionSets(conditionalText);
            conditionals.ConditionSets.AddRange(conditionSets);

            currentPosition = bracketEnd + 1;
        }

        // Parse remaining arguments (everything after the last bracket)
        var remainingText = text.Substring(currentPosition).Trim();
        if (!string.IsNullOrWhiteSpace(remainingText))
        {
            arguments.Add(new CommandArgument { Value = remainingText });
        }

        return conditionals.ConditionSets.Count > 0 ? (conditionals, arguments) : (null, arguments);
    }

    private List<ConditionSet> ParseConditionSets(string conditionalText)
    {
        var conditionSets = new List<ConditionSet>();
        
        // Split by semicolons for OR logic (different condition sets within the same bracket)
        var orParts = conditionalText.Split(';', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var orPart in orParts)
        {
            var trimmedOrPart = orPart.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedOrPart))
            {
                var conditionSet = new ConditionSet();
                
                // Split by commas for AND logic (conditions within a set)
                var andParts = trimmedOrPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var andPart in andParts)
                {
                    var trimmedAndPart = andPart.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedAndPart))
                    {
                        var condition = ParseCondition(trimmedAndPart);
                        if (condition != null)
                        {
                            conditionSet.Conditions.Add(condition);
                        }
                    }
                }
                
                if (conditionSet.Conditions.Count > 0)
                {
                    conditionSets.Add(conditionSet);
                }
            }
        }

        return conditionSets;
    }

    private Condition? ParseCondition(string conditionText)
    {
        var trimmed = conditionText.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return null;

        // Check if it's a key:value format
        var colonIndex = trimmed.IndexOf(':');
        if (colonIndex > 0)
        {
            var key = trimmed.Substring(0, colonIndex).Trim();
            var value = trimmed.Substring(colonIndex + 1).Trim();
            return new Condition { Key = key, Value = value };
        }
        else
        {
            // Just a key without value
            return new Condition { Key = trimmed, Value = null };
        }
    }

    private int FindMatchingBracket(string text, int startIndex)
    {
        var bracketCount = 0;
        for (int i = startIndex; i < text.Length; i++)
        {
            if (text[i] == '[')
                bracketCount++;
            else if (text[i] == ']')
            {
                bracketCount--;
                if (bracketCount == 0)
                    return i;
            }
        }
        return -1; // No matching bracket found
    }

    /// <summary>
    /// Validates macro text and returns any syntax errors
    /// </summary>
    /// <param name="macroText">The macro text to validate</param>
    /// <returns>List of validation errors</returns>
    public List<string> ValidateMacroText(string macroText)
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
    
    private bool IsValidDirective(string directive)
    {
        var validDirectives = new[]
        {
            "#showtooltip", "#show", "#hide", "#icon"
        };
        
        return validDirectives.Any(valid => directive.StartsWith(valid, StringComparison.OrdinalIgnoreCase));
    }
    
    private bool IsValidSlashCommand(string command)
    {
        // Basic validation for common slash commands
        var validCommands = new[]
        {
            "/cast", "/use", "/target", "/focus", "/assist", "/follow", "/stopcasting",
            "/cancelaura", "/cancelform", "/cancelbuff", "/cleartarget", "/dismount",
            "/equip", "/equipslot", "/invite", "/kick", "/leave", "/macro", "/petattack",
            "/petfollow", "/petstay", "/petpassive", "/petdefensive", "/petaggressive",
            "/random", "/roll", "/say", "/yell", "/whisper", "/party", "/raid", "/guild",
            "/emote", "/dance", "/sit", "/stand", "/sleep", "/mount", "/dismount"
        };
        
        return validCommands.Any(valid => command.StartsWith(valid, StringComparison.OrdinalIgnoreCase));
    }

    public List<string> ValidateMacro(Macro macro)
    {
        var errors = new List<string>();

        foreach (var line in macro.Lines)
        {
            switch (line)
            {
                case DirectiveLine d:
                    errors.AddRange(ValidateDirective(d));
                    break;
                case CommandLine c:
                    errors.AddRange(ValidateCommand(c));
                    break;
                case CommentLine:
                    // Comments are always valid
                    break;
            }
        }

        return errors;
    }

    private List<string> ValidateDirective(DirectiveLine directive)
    {
        var errors = new List<string>();
        
        var validDirectives = new[] { "#showtooltip", "#show", "#hide", "#icon" };
        if (!validDirectives.Contains(directive.Directive, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add($"Invalid directive: {directive.Directive}");
        }

        return errors;
    }

    private List<string> ValidateCommand(CommandLine command)
    {
        var errors = new List<string>();

        // Validate command name using the new CommandValidator
        if (!CommandValidator.IsValidCommand(command.Command))
        {
            errors.Add($"Invalid command: {command.Command}");
        }

        // Validate conditionals
        if (command.Conditionals != null)
        {
            errors.AddRange(ConditionalValidator.ValidateConditional(command.Conditionals));
        }

        return errors;
    }
} 