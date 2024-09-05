namespace EasyWowMacro.Business.Models;

public class ModifierGroup
{
    public List<Modifier>? Modifiers { get; set; }
    
    public override string ToString()
    {
        return Modifiers != null ? $"[{string.Join(",", Modifiers.Select(c => c.ToString()))}]" : string.Empty;
    }
}