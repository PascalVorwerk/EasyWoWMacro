using System.Text;

namespace EasyWoWMacro.Business.Models;

public class Macro
{
    public List<Command> Commands { get; } = [];

    public void AddCommand(Command command)
    {
        Commands.Add(command);
    }
    
    public void RemoveCommand(Command command)
    {
        Commands.Remove(command);
    }
    
    public void ClearCommands()
    {
        Commands.Clear();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var command in Commands)
        {
            sb.AppendLine("/" + command);
        }
        
        return sb.ToString().TrimEnd();
    }
}