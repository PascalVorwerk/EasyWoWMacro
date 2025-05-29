namespace EasyWoWMacro.Business.Models;

public abstract class Command(string verb, string displayName, string description)
{
    public string Verb { get; set; } = verb;
    public string DisplayName { get; set; } = displayName;
    public string Description { get; set; } = description;
}

public class SecureCommand(string verb, string displayName, string description) : Command(verb, displayName, description)
{
    public List<CommandClause> Clauses { get; set; } = new List<CommandClause>();
}

public class InSecureCommand(string verb, string displayName, string description) : Command(verb, displayName, description)
{
    public string Parameter { get; set; } = string.Empty;
}