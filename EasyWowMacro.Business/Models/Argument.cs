namespace EasyWowMacro.Business.Models;

public class Argument
{
    public List<ModifierGroup> ModifierGroups { get; set; } = new();
    public string ArgumentValue { get; set; } = string.Empty;
    
    public override string ToString()
    {
        var parts = new List<string> { };

        if (ModifierGroups.Any())
        {
            var modifierParts = ModifierGroups
                .Select(mp => mp + "")
                .ToList();

            parts.AddRange(modifierParts);
        }
        
        parts.Add(ArgumentValue);
        
        return string.Join("", parts).Trim();
    }
}