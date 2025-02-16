using System.Text;

namespace EasyWoWMacro.Business.Models;

public class Macro
{
    public string Value { get; set; } = string.Empty;
    public List<MacroCommand> Commands { get; set; } = new();
    
    public override string ToString() => ToString(trim: false);
    
    public string ToString(bool trim)
    {
        var sb = new StringBuilder();
        foreach (var command in Commands)
        {
            // Each command is prefixed with a slash.
            sb.Append($"/{command.ToString(trim)} ");
        }
        return trim ? sb.ToString().Trim() : sb.ToString();
    }
}