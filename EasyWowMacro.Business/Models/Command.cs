namespace EasyWowMacro.Business.Models;

public class Command
{
    public required string Name { get; set; }
    public required string CommandType { get; set; }
    public List<Argument> Arguments { get; set; } = new();

    
    public override string ToString()
    {
        var parts = new List<string> { CommandType + " " };

        if (Arguments.Any())
        {
            var modifierParts = Arguments
                .Select(mp => mp + ";")
                .ToList();

            parts.AddRange(modifierParts);
        }

        return string.Join("", parts).Trim();
    }

}