
namespace EasyWoWMacro.Business.Models
{
    public class Macro
    {
        public List<Command> Commands { get; set; } = new();

        public override string ToString() => string.Join("\n",Commands);
    }

    public class Command
    {
        public CommandVerb CommandVerb { get; set; } = default!;
        public List<CommandObject> CommandObjects { get; set; } = new();

        public override string ToString() =>
            $"/{CommandVerb} " + string.Join(";", CommandObjects);
    }

    // CommandVerb class with additional fields for name and description
    public class CommandVerb
    {
        public string DisplayName { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public CommandVerb(string displayName, string name, string description)
        {
            DisplayName = displayName;
            Name = name;
            Description = description;
        }

        public override string ToString() => Name;
    }

    public class CommandObject
    {
        public List<Condition> Conditions { get; set; } = new();
        public List<Parameter> Parameters { get; set; } = new();

        public override string ToString()
        {
            var conditions = Conditions.Any() ? $"[{string.Join(",", Conditions)}]" : string.Empty;
            var parameters = string.Join(" ", Parameters);
            return $"{conditions} {parameters}".Trim();
        }
    }

    public class Condition
    {
        public List<ConditionPhrase> ConditionPhrases { get; set; } = new();

        public override string ToString() => string.Join(",", ConditionPhrases);
    }

    public class ConditionPhrase
    {
        public bool IsNegated { get; set; } = false;
        public OptionWord? Option { get; set; }
        public List<OptionArgument> OptionArguments { get; set; } = new();
        public Target? Target { get; set; }

        public override string ToString()
        {
            var negation = IsNegated ? "no " : string.Empty;
            var optionArguments = OptionArguments.Any()
                ? $":{string.Join("/", OptionArguments)}"
                : string.Empty;
            var optionPart = Option != null ? $"{negation}{Option}{optionArguments}" : string.Empty;
            var targetPart = Target != null ? $"@{Target}" : string.Empty;
            return $"{optionPart}{targetPart}".Trim();
        }
    }

    // OptionWord with additional fields for name and description
    public class OptionWord
    {
        public string DisplayName { get; private set; }
        public string Value { get; private set; }
        public string Description { get; private set; }

        public OptionWord(string displayName, string value, string description)
        {
            DisplayName = displayName;
            Value = value;
            Description = description;
        }

        public override string ToString() => Value;
    }

    // OptionArgument with additional fields for name and description
    public class OptionArgument
    {
        public string DisplayName { get; private set; }
        public string Value { get; private set; }
        public string Description { get; private set; }

        public OptionArgument(string displayName, string value, string description)
        {
            DisplayName = displayName;
            Value = value;
            Description = description;
        }

        public override string ToString() => Value;
    }
    public class Parameter
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString() => Value;
    }

    // Target with additional fields for name and description
    public class Target
    {
        public string DisplayName { get; private set; }
        public string Pattern { get; private set; }
        public string Description { get; private set; }

        public Target(string displayName, string pattern, string description)
        {
            DisplayName = displayName;
            Pattern = pattern;
            Description = description;
        }

        public override string ToString() => Pattern;
    }
}
