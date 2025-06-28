namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Abstract base class for a line in a macro (directive, command, comment)
/// </summary>
public abstract class MacroLine
{
    public int LineNumber { get; set; }
    public string RawText { get; set; } = string.Empty;
} 