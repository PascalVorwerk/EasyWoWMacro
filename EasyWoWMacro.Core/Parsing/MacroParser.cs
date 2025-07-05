using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing.Utilities;
using EasyWoWMacro.Core.Validation;
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

    private static CommandLine ParseCommandLine(string line, int lineNumber)
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
            Clauses = new List<CommandClause>()
        };

        // Parse clauses (semicolon-separated conditional logic)
        if (!string.IsNullOrWhiteSpace(rest))
        {
            commandLine.Clauses = ParseCommandClauses(rest);
        }

        return commandLine;
    }

    private static List<CommandClause> ParseCommandClauses(string rest)
    {
        var clauses = new List<CommandClause>();

        // Split by semicolons outside brackets to get individual clauses
        var semicolonParts = StringUtilities.SplitBySemicolonsOutsideBrackets(rest);
        
        foreach (var part in semicolonParts)
        {
            var trimmedPart = part.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedPart))
            {
                var (conditionals, arguments) = ConditionalParser.ParseConditionalsAndArguments(trimmedPart);
                
                // Create a clause with the conditions and first argument (WoW only allows one argument per clause)
                var clause = new CommandClause
                {
                    Conditions = conditionals,
                    Argument = arguments.Count > 0 ? arguments[0] : string.Empty
                };
                
                clauses.Add(clause);
            }
        }

        return clauses;
    }

    /// <summary>
    /// Validates macro text and returns any syntax errors
    /// </summary>
    /// <param name="macroText">The macro text to validate</param>
    /// <returns>List of validation errors</returns>
    public List<string> ValidateMacroText(string macroText)
    {
        return SyntaxValidator.ValidateMacroText(macroText);
    }

    /// <summary>
    /// Validates bracket syntax in a command line
    /// </summary>
    /// <param name="line">The line to validate</param>
    /// <returns>List of validation errors</returns>
    public List<string> ValidateBrackets(string line)
    {
        return SyntaxValidator.ValidateBrackets(line);
    }

    /// <summary>
    /// Validates a parsed macro object
    /// </summary>
    /// <param name="macro">The macro to validate</param>
    /// <returns>List of validation errors</returns>
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

    private static List<string> ValidateDirective(DirectiveLine directive)
    {
        var errors = new List<string>();
        
        if (!WoWMacroConstants.ValidDirectives.Contains(directive.Directive, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add($"Invalid directive: {directive.Directive}");
        }

        return errors;
    }

    private static List<string> ValidateCommand(CommandLine command)
    {
        var errors = new List<string>();

        // Validate command name using the CommandValidator
        if (!CommandValidator.IsValidCommand(command.Command))
        {
            errors.Add($"Invalid command: {command.Command}");
        }

        // Validate conditionals in clauses
        foreach (var clause in command.Clauses)
        {
            if (clause.Conditions != null)
            {
                errors.AddRange(ConditionalValidator.ValidateConditional(clause.Conditions));
            }
        }

        return errors;
    }
} 