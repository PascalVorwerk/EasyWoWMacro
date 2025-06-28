namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a directive line in a macro (e.g., #showtooltip)
/// </summary>
public class DirectiveLine : MacroLine
{
    public string Directive { get; set; } = string.Empty;
    public string? Argument { get; set; }
} 