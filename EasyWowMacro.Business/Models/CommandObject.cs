using System.Text;

namespace EasyWoWMacro.Business.Models;

public class CommandObject
{
    public List<Condition> Conditions { get; set; } = new();
    public string Parameters { get; set; } = null!;
    
    public override string ToString() => ToString(trim: false);

    public string ToString(bool trim)
    {
        var sb = new StringBuilder();
        // If conditions exist, output them in brackets.
        if (Conditions.Any())
        {
            sb.Append(string.Join("", Conditions.Select(c => c.ToString(trim))));
            sb.Append(" ");
        }
        // Append parameters (trimming extra whitespace if needed).
        sb.Append(trim ? Parameters.Trim().Replace(" ", "") : Parameters);
        return sb.ToString().Trim();
    }
}