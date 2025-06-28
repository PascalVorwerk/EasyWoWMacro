namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a conditional block in a WoW macro (e.g., [mod:shift,@focus] or [mod:alt][@target])
/// </summary>
public class Conditional
{
    /// <summary>
    /// Multiple condition sets separated by semicolons (OR logic)
    /// Each set contains conditions separated by commas (AND logic)
    /// </summary>
    public List<ConditionSet> ConditionSets { get; set; } = new();
}

/// <summary>
/// Represents a set of conditions that must ALL be true (AND logic)
/// </summary>
public class ConditionSet
{
    public List<Condition> Conditions { get; set; } = new();
}

/// <summary>
/// Represents a single condition (e.g., mod:shift, @focus, combat, etc.)
/// </summary>
public class Condition
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    
    /// <summary>
    /// Gets the full condition string representation
    /// </summary>
    public override string ToString()
    {
        return Value != null ? $"{Key}:{Value}" : Key;
    }
} 