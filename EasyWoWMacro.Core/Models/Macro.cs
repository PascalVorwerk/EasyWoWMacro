using EasyWoWMacro.Core.Parsing;

namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a World of Warcraft macro
/// </summary>
public class Macro
{
    /// <summary>
    /// Gets or sets the name of the macro
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the icon name for the macro
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of commands in the macro
    /// </summary>
    public List<MacroLine> Lines { get; set; } = new();
    
    /// <summary>
    /// Gets the formatted macro text that can be copied into World of Warcraft
    /// </summary>
    public string GetFormattedMacro()
    {
        var lines = new List<string>();
        var parser = new MacroParser();
        
        // Add name and icon line
        if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Icon))
        {
            var nameIconLine = "#showtooltip";
            if (!string.IsNullOrEmpty(Icon))
            {
                nameIconLine += $" {Icon}";
            }
            lines.Add(nameIconLine);
        }
        
        // Add macro lines, skipping lines with bracket errors
        foreach (var line in Lines)
        {
            // Only check CommandLine and DirectiveLine for bracket errors
            string raw = line.RawText;
            if ((line is CommandLine || line is DirectiveLine) && parser.ValidateBrackets(raw).Count > 0)
            {
                // Skip this line, or optionally add a marker
                continue;
            }
            switch (line)
            {
                case DirectiveLine d:
                    lines.Add(d.Argument != null ? $"{d.Directive} {d.Argument}" : d.Directive);
                    break;
                case CommandLine c:
                    if (c.Clauses != null && c.Clauses.Count > 0)
                    {
                        var clauseStrings = new List<string>();
                        foreach (var (conds, arg) in c.Clauses)
                        {
                            var clause = "";
                            if (conds != null && conds.ConditionSets.Count > 0)
                            {
                                foreach (var conditionSet in conds.ConditionSets)
                                {
                                    var conditions = conditionSet.Conditions.Select(cond => cond.ToString());
                                    clause += $"[{string.Join(",", conditions)}]";
                                }
                                clause += " ";
                            }
                            if (arg != null && !string.IsNullOrWhiteSpace(arg.Value))
                            {
                                clause += arg.Value.Trim();
                            }
                            clauseStrings.Add(clause.TrimEnd());
                        }
                        lines.Add($"{c.Command} {string.Join("; ", clauseStrings).Trim()}");
                    }
                    else
                    {
                        // fallback to old logic if no clauses
                        var commandLine = c.Command;
                        var hasConditionals = c.Conditionals != null && c.Conditionals.ConditionSets.Count > 0;
                        if (hasConditionals)
                        {
                            var conditionalStrings = new List<string>();
                            foreach (var conditionSet in c.Conditionals.ConditionSets)
                            {
                                var conditions = conditionSet.Conditions.Select(cond => cond.ToString());
                                conditionalStrings.Add($"[{string.Join(",", conditions)}]");
                            }
                            commandLine += " " + string.Join("", conditionalStrings);
                        }
                        var args = c.Arguments != null && c.Arguments.Count > 0 ? " " + string.Join(" ", c.Arguments.Select(a => a.Value)) : string.Empty;
                        lines.Add($"{commandLine}{args}");
                    }
                    break;
                case CommentLine cm:
                    lines.Add($"; {cm.Comment}");
                    break;
                default:
                    lines.Add(line.RawText);
                    break;
            }
        }
        
        return string.Join(Environment.NewLine, lines);
    }
    
    /// <summary>
    /// Validates the macro and returns any validation errors
    /// </summary>
    public List<string> Validate()
    {
        var errors = new List<string>();
        
        if (Lines.Count == 0)
        {
            errors.Add("Macro must contain at least one command");
        }
        
        // Use the dedicated parser for validation
        var parser = new MacroParser();
        var macroErrors = parser.ValidateMacro(this);
        errors.AddRange(macroErrors);
        
        return errors;
    }
} 