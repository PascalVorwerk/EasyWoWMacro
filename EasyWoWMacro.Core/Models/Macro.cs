using System.Collections.Generic;

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
        
        // Add macro lines
        foreach (var line in Lines)
        {
            switch (line)
            {
                case DirectiveLine d:
                    lines.Add(d.Argument != null ? $"{d.Directive} {d.Argument}" : d.Directive);
                    break;
                case CommandLine c:
                    var args = c.Arguments != null && c.Arguments.Count > 0 ? " " + string.Join(" ", c.Arguments.Select(a => a.Value)) : string.Empty;
                    lines.Add($"{c.Command}{args}");
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
        
        // Check for common macro syntax issues
        foreach (var line in Lines)
        {
            if (line is CommandLine c && c.Command.StartsWith("/") && !IsValidSlashCommand(c.Command))
            {
                errors.Add($"Invalid slash command: {c.Command}");
            }
        }
        
        return errors;
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
} 