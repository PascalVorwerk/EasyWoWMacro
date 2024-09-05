namespace EasyWowMacro.Business.Models;

public class Modifier
{
    public required string Name { get; set; }
    public required string Value { get; set; }
    public bool IsInverse { get; set; }
    public bool IsTargetModifier { get; set; } = false;
    public bool IsKeyModifier { get; set; } = false;
    public override string ToString()
    {
        if ((IsTargetModifier || IsKeyModifier) && IsInverse)
        {
            throw new InvalidOperationException($"Modifier '{Name}' cannot be inversed.");
        }
        
        return IsInverse ? $"no{Value}" : Value;
    }
}