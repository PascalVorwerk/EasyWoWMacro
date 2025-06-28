namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Represents a comment line in a macro (e.g., ; This is a comment)
/// </summary>
public class CommentLine : MacroLine
{
    public string Comment { get; set; } = string.Empty;
} 