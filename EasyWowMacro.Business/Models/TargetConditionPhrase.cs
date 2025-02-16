namespace EasyWoWMacro.Business.Models;

public class TargetConditionPhrase : ConditionPhrase
{
    public string TargetPattern { get; set; }
    
    public override string ToString() => ToString(trim: false);

    public override string ToString(bool trim)
    {
        return "@" + (trim ? TargetPattern.Trim().Replace(" ", "") : TargetPattern);
    }
}