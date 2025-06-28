namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a command line in a macro (e.g., /cast [mod:shift] Polymorph)
/// </summary>
public class CommandLine : MacroLine
{
    public string Command { get; set; } = string.Empty;
    public List<CommandArgument> Arguments { get; set; } = new();
    public Conditional? Conditionals { get; set; }

    // New: List of (Conditional, CommandArgument) pairs for each clause
    public List<(Conditional? Conditionals, CommandArgument? Argument)> Clauses { get; set; } = new();
} 