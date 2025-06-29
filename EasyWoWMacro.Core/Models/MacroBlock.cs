namespace EasyWoWMacro.Core.Models;

public enum BlockType
{
    Directive,
    Command,
    Conditional,
    ConditionalGroup,
    Argument
}

public class MacroBlock
{
    public string Content { get; set; } = string.Empty;
    public BlockType Type { get; set; }

    public MacroBlock() { }
    public MacroBlock(string content, BlockType type)
    {
        Content = content;
        Type = type;
    }
} 