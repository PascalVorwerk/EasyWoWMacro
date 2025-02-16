using System.Text;

namespace EasyWoWMacro.Business.Models;

public class OptionConditionPhrase : ConditionPhrase
{
    public bool IsNegated { get; set; }
    public string OptionWord { get; set; } = null!;
    public List<string> Arguments { get; set; } = new();
    
    public override string ToString() => ToString(trim: false);
    
    public override string ToString(bool trim)
    {
        var sb = new StringBuilder();
        if (IsNegated) sb.Append("no");
        sb.Append(OptionWord);
        if (Arguments.Any())
        {
            sb.Append(':');
            // Optionally trim each argument.
            var args = Arguments.Select(arg => trim ? arg.Trim() : arg);
            sb.Append(string.Join("/", args));
        }
        return sb.ToString();
    }
}