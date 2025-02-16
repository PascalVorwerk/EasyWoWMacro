using System.Text;

namespace EasyWoWMacro.Business.Models;

public class MacroCommand
{
    public string Verb { get; set; }
    public List<CommandObject> Objects { get; set; } = new();
    
    public override string ToString() => ToString(trim: false);

    public string ToString(bool trim)
    {
        var sb = new StringBuilder();
        sb.Append(Verb);
        // Append each command object separated by a semicolon.
        if (Objects.Any())
        {
            sb.Append(' ');
            sb.Append(string.Join("; ", Objects.Select(o => o.ToString(trim))));
        }
        return sb.ToString().Trim();
    }
}