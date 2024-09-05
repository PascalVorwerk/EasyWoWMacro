namespace EasyWowMacro.Business.Models;

public class Macro
{
    public string? Name { get; set; }
    public List<Command> Commands { get; set; } = new List<Command>();

    public override string ToString()
    {
        return string.Join("\n", Commands.Select(c => c.ToString()));
    }
}