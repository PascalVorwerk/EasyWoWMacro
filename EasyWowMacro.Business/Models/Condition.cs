using System.Text;

namespace EasyWoWMacro.Business.Models;

public class Condition
{
    public List<ConditionPhrase> Phrases { get; set; } = new();
    
    public override string ToString() => ToString(trim: false);

    public string ToString(bool trim)
    {
        if (Phrases.Count == 0) return string.Empty;
        var sb = new StringBuilder();
        sb.Append('[');
        // Join each phrase with a comma.
        sb.Append(string.Join(",", Phrases.Select(p => p.ToString(trim))));
        sb.Append(']');
        return sb.ToString();
    }
    
}