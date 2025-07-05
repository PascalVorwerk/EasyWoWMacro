namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a command line in a macro (e.g., /cast [mod:shift] Polymorph; [combat] Fireball; Frostbolt)
/// A command line consists of a command followed by semicolon-separated clauses
/// </summary>
public class CommandLine : MacroLine
{
    /// <summary>
    /// The slash command (e.g., "/cast", "/use", "/target")
    /// </summary>
    public string Command { get; set; } = string.Empty;
    
    /// <summary>
    /// List of conditional clauses separated by semicolons (OR logic)
    /// Each clause represents: [conditions] argument
    /// WoW evaluates clauses in order and executes the first one whose conditions are met
    /// </summary>
    public List<CommandClause> Clauses { get; set; } = new();
}

/// <summary>
/// Represents a single clause in a command line (e.g., [mod:shift] Polymorph)
/// </summary>
public class CommandClause
{
    /// <summary>
    /// Optional conditions that must be met for this clause to execute
    /// If null, this clause always executes (typically used as fallback)
    /// </summary>
    public Conditional? Conditions { get; set; }
    
    /// <summary>
    /// The argument/spell/item to execute if conditions are met
    /// </summary>
    public string Argument { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets a string representation of this clause
    /// </summary>
    public override string ToString()
    {
        if (Conditions != null && Conditions.ConditionSets.Any())
        {
            return $"[{string.Join("][", Conditions.ConditionSets.Select(cs => string.Join(",", cs.Conditions.Select(c => c.ToString()))))}] {Argument}";
        }
        return Argument;
    }
} 