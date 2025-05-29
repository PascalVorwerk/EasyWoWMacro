namespace EasyWoWMacro.Business.Models;

public class CommandClause
{
    public List<ConditionGroup> Conditions { get; set; } = new List<ConditionGroup>();
    public string Parameters { get; set; } = string.Empty;
}