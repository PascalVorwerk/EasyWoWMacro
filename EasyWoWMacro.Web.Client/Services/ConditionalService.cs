using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Validation;
using EasyWoWMacro.Web.Client.Models;

namespace EasyWoWMacro.Web.Client.Services;

/// <summary>
/// Implementation of IConditionalService that uses core validation classes
/// </summary>
public class ConditionalService : IConditionalService
{
    // Mapping of conditional prefixes to their configuration keys
    private static readonly Dictionary<string, string> ConditionalToConfigKey = new()
    {
        ["mod:"] = "modifier",
        ["nomod:"] = "modifier",
        ["stance:"] = "stance",
        ["equipped:"] = "equippedType", // Special handling needed
        ["worn:"] = "equippedType", // Same as equipped
        ["form:"] = "form",
        ["button:"] = "button",
        ["btn:"] = "button", // Alias for button
        ["threat:"] = "threat",
        ["spec:"] = "spec",
        ["talent:"] = "talent",
        ["glyph:"] = "glyph",
        ["spell:"] = "spell",
        ["item:"] = "item",
        ["aura:"] = "aura",
        ["buff:"] = "buff",
        ["debuff:"] = "buff", // Uses same config key as buff
        ["actionbar:"] = "actionbar",
        ["bar:"] = "actionbar", // Alias for actionbar
        ["vehicle:"] = "vehicle",
        ["canexitvehicle:"] = "canexitvehicle",
        ["channeling:"] = "channeling",
        ["casting:"] = "casting"
    };

    // Common basic conditionals that users frequently use
    private static readonly HashSet<string> CommonBasicConditionals =
    [
        "combat", "nocombat", "harm", "help", "dead", "nodead",
        "stealth", "nostealth", "mounted", "nomounted", "flying", "noflying",
        "indoors", "outdoors", "group", "raid", "party", "solo", "pet", "nopet",
        "channeling", "casting", "nocasting", "exists", "noexists",
        "@target", "@mouseover", "@focus", "@player", "@cursor"
    ];

    // Advanced conditional prefixes that require configuration
    private static readonly HashSet<string> AdvancedConditionalPrefixes =
    [
        "mod:", "nomod:", "stance:", "equipped:", "worn:", "form:", "button:", "btn:",
        "threat:", "spec:", "talent:", "glyph:", "spell:", "item:", "aura:", "buff:", "debuff:",
        "actionbar:", "bar:", "vehicle:", "canexitvehicle:", "channeling:", "casting:"
    ];

    public IEnumerable<string> GetValidConditionalKeys()
    {
        return ConditionalValidator.GetValidConditionalKeys();
    }

    public IEnumerable<string> GetValidValuesForKey(string key)
    {
        return ConditionalValidator.GetValidValuesForKey(key);
    }

    public bool IsValidCondition(string key, string? value = null)
    {
        var condition = new Condition { Key = key, Value = value };
        return ConditionalValidator.IsValidCondition(condition);
    }

    public string FormatConditional(string key, string? value = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            return key;

        return $"{key}:{value}";
    }

    public List<string> ValidateConditionalString(string conditionalString)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(conditionalString))
            return errors;

        var conditions = conditionalString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .Where(c => !string.IsNullOrEmpty(c));

        foreach (var condition in conditions)
        {
            var colonIndex = condition.IndexOf(':');
            if (colonIndex > 0)
            {
                var key = condition.Substring(0, colonIndex).Trim();
                var value = condition.Substring(colonIndex + 1).Trim();

                if (!IsValidCondition(key, value))
                {
                    errors.Add($"Invalid conditional: {condition}");
                }
            }
            else
            {
                if (!IsValidCondition(condition))
                {
                    errors.Add($"Invalid conditional: {condition}");
                }
            }
        }

        return errors;
    }

    public IEnumerable<string> GetBasicConditions()
    {
        // Return common basic conditionals that don't require values
        return CommonBasicConditionals.OrderBy(k => k);
    }

    public IEnumerable<string> GetAdvancedConditions()
    {
        // Return advanced conditional prefixes that require configuration
        return AdvancedConditionalPrefixes.OrderBy(k => k);
    }

    /// <summary>
    /// Gets all available basic conditionals (for search functionality)
    /// </summary>
    public IEnumerable<string> GetAllBasicConditions()
    {
        return GetValidConditionalKeys()
            .Where(key => !AdvancedConditionalPrefixes.Contains(key) && !key.EndsWith(":"))
            .OrderBy(k => k);
    }

    public string FormatConditionalsWithConfiguration(IEnumerable<string> conditions, Dictionary<string, string> configuration)
    {
        var conditionalArray = conditions as string[] ?? conditions.ToArray();
        if (!conditionalArray.Any())
            return "[No conditions selected]";

        var formattedConditions = conditionalArray.Select(condition => FormatSingleConditionalWithConfiguration(condition, configuration)).Where(formattedCondition => !string.IsNullOrEmpty(formattedCondition)).ToList();

        return $"[{string.Join(", ", formattedConditions)}]";
    }

    public string? GetConfigurationKeyForConditional(string conditional)
    {
        return ConditionalToConfigKey.GetValueOrDefault(conditional);
    }

    public IEnumerable<ConditionalConfigurationInfo> GetConditionalConfigurationInfo()
    {
        return new List<ConditionalConfigurationInfo>
        {
            new()
            {
                Conditional = "mod:",
                ConfigurationKey = "modifier",
                Label = "Modifier Key",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "alt", Label = "Alt" },
                    new SelectOption { Value = "ctrl", Label = "Ctrl" },
                    new SelectOption { Value = "shift", Label = "Shift" },
                    new SelectOption { Value = "alt,ctrl", Label = "Alt+Ctrl" },
                    new SelectOption { Value = "alt,shift", Label = "Alt+Shift" },
                    new SelectOption { Value = "ctrl,shift", Label = "Ctrl+Shift" },
                    new SelectOption { Value = "alt,ctrl,shift", Label = "Alt+Ctrl+Shift" }
                ]
            },
            new()
            {
                Conditional = "nomod:",
                ConfigurationKey = "modifier",
                Label = "Modifier Key",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "alt", Label = "Alt" },
                    new SelectOption { Value = "ctrl", Label = "Ctrl" },
                    new SelectOption { Value = "shift", Label = "Shift" },
                    new SelectOption { Value = "alt,ctrl", Label = "Alt+Ctrl" },
                    new SelectOption { Value = "alt,shift", Label = "Alt+Shift" },
                    new SelectOption { Value = "ctrl,shift", Label = "Ctrl+Shift" },
                    new SelectOption { Value = "alt,ctrl,shift", Label = "Alt+Ctrl+Shift" }
                ]
            },
            new()
            {
                Conditional = "stance:",
                ConfigurationKey = "stance",
                Label = "Stance/Form",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Battle Stance", Group = "Warrior Stances" },
                    new SelectOption { Value = "2", Label = "2 - Defensive Stance", Group = "Warrior Stances" },
                    new SelectOption { Value = "3", Label = "3 - Berserker Stance", Group = "Warrior Stances" },
                    new SelectOption { Value = "bear", Label = "Bear Form", Group = "Druid Forms" },
                    new SelectOption { Value = "aquatic", Label = "Aquatic Form", Group = "Druid Forms" },
                    new SelectOption { Value = "cat", Label = "Cat Form", Group = "Druid Forms" },
                    new SelectOption { Value = "travel", Label = "Travel Form", Group = "Druid Forms" },
                    new SelectOption { Value = "moonkin", Label = "Moonkin Form", Group = "Druid Forms" },
                    new SelectOption { Value = "stealth", Label = "Stealth (Rogue)", Group = "Other" },
                    new SelectOption { Value = "shadowform", Label = "Shadowform (Priest)", Group = "Other" }
                ]
            },
            new()
            {
                Conditional = "equipped:",
                ConfigurationKey = "equippedType",
                Label = "Equipment Slot/Item",
                Type = ConfigurationType.EquipmentSlot,
                Description = "Check if specific item/slot is equipped"
            },
            new()
            {
                Conditional = "worn:",
                ConfigurationKey = "equippedType",
                Label = "Equipment Slot/Item",
                Type = ConfigurationType.EquipmentSlot,
                Description = "Check if specific item/slot is equipped"
            },
            new()
            {
                Conditional = "form:",
                ConfigurationKey = "form",
                Label = "Form Type",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "bear", Label = "Bear Form" },
                    new SelectOption { Value = "cat", Label = "Cat Form" },
                    new SelectOption { Value = "travel", Label = "Travel Form" },
                    new SelectOption { Value = "aquatic", Label = "Aquatic Form" },
                    new SelectOption { Value = "moonkin", Label = "Moonkin Form" },
                    new SelectOption { Value = "tree", Label = "Tree of Life Form" }
                ],
                Description = "Check current druid form"
            },
            new()
            {
                Conditional = "button:",
                ConfigurationKey = "button",
                Label = "Mouse Button",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Left Click" },
                    new SelectOption { Value = "2", Label = "2 - Right Click" },
                    new SelectOption { Value = "3", Label = "3 - Middle Click" },
                    new SelectOption { Value = "4", Label = "4 - Button 4" },
                    new SelectOption { Value = "5", Label = "5 - Button 5" }
                ],
                Description = "Check which mouse button was pressed"
            },
            new()
            {
                Conditional = "btn:",
                ConfigurationKey = "button",
                Label = "Mouse Button",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Left Click" },
                    new SelectOption { Value = "2", Label = "2 - Right Click" },
                    new SelectOption { Value = "3", Label = "3 - Middle Click" },
                    new SelectOption { Value = "4", Label = "4 - Button 4" },
                    new SelectOption { Value = "5", Label = "5 - Button 5" }
                ],
                Description = "Check which mouse button was pressed"
            },
            new()
            {
                Conditional = "threat:",
                ConfigurationKey = "threat",
                Label = "Threat Level",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Low" },
                    new SelectOption { Value = "2", Label = "2 - Medium" },
                    new SelectOption { Value = "3", Label = "3 - High" }
                ],
                Description = "Check threat level on target"
            },
            new()
            {
                Conditional = "spec:",
                ConfigurationKey = "spec",
                Label = "Specialization",
                Placeholder = "Spec name or number (e.g., 'Fire', '1')",
                Type = ConfigurationType.Text,
                Description = "Check current specialization"
            },
            new()
            {
                Conditional = "talent:",
                ConfigurationKey = "talent",
                Label = "Talent",
                Placeholder = "Talent name or ID",
                Type = ConfigurationType.Text,
                Description = "Check if specific talent is learned"
            },
            new()
            {
                Conditional = "glyph:",
                ConfigurationKey = "glyph",
                Label = "Glyph",
                Placeholder = "Glyph name",
                Type = ConfigurationType.Text,
                Description = "Check if specific glyph is active"
            },
            new()
            {
                Conditional = "spell:",
                ConfigurationKey = "spell",
                Label = "Spell",
                Placeholder = "Spell name",
                Type = ConfigurationType.Text,
                Description = "Check if specific spell is known"
            },
            new()
            {
                Conditional = "item:",
                ConfigurationKey = "item",
                Label = "Item",
                Placeholder = "Item name",
                Type = ConfigurationType.Text,
                Description = "Check if specific item is in bags"
            },
            new()
            {
                Conditional = "aura:",
                ConfigurationKey = "aura",
                Label = "Aura",
                Placeholder = "Aura/buff/debuff name",
                Type = ConfigurationType.Text,
                Description = "Check if specific aura is active"
            },
            new()
            {
                Conditional = "buff:",
                ConfigurationKey = "buff",
                Label = "Buff/Debuff",
                Placeholder = "Buff/debuff name",
                Type = ConfigurationType.Text,
                Description = "Check if specific buff/debuff is active"
            },
            new()
            {
                Conditional = "debuff:",
                ConfigurationKey = "buff",
                Label = "Buff/Debuff",
                Placeholder = "Buff/debuff name",
                Type = ConfigurationType.Text,
                Description = "Check if specific buff/debuff is active"
            },
            new()
            {
                Conditional = "actionbar:",
                ConfigurationKey = "actionbar",
                Label = "Action Bar",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Main Action Bar" },
                    new SelectOption { Value = "2", Label = "2 - Bottom Left Action Bar" },
                    new SelectOption { Value = "3", Label = "3 - Bottom Right Action Bar" },
                    new SelectOption { Value = "4", Label = "4 - Right Action Bar" },
                    new SelectOption { Value = "5", Label = "5 - Right Action Bar 2" },
                    new SelectOption { Value = "6", Label = "6 - Extra Action Bar" }
                ],
                Description = "Check current action bar"
            },
            new()
            {
                Conditional = "bar:",
                ConfigurationKey = "actionbar",
                Label = "Action Bar",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Main Action Bar" },
                    new SelectOption { Value = "2", Label = "2 - Bottom Left Action Bar" },
                    new SelectOption { Value = "3", Label = "3 - Bottom Right Action Bar" },
                    new SelectOption { Value = "4", Label = "4 - Right Action Bar" },
                    new SelectOption { Value = "5", Label = "5 - Right Action Bar 2" },
                    new SelectOption { Value = "6", Label = "6 - Extra Action Bar" }
                ],
                Description = "Check current action bar"
            },
            new()
            {
                Conditional = "vehicle:",
                ConfigurationKey = "vehicle",
                Label = "Vehicle",
                Placeholder = "Vehicle name",
                Type = ConfigurationType.Text,
                Description = "Check if in specific vehicle"
            },
            new()
            {
                Conditional = "canexitvehicle:",
                ConfigurationKey = "canexitvehicle",
                Label = "Can Exit Vehicle",
                Placeholder = "Vehicle name",
                Type = ConfigurationType.Text,
                Description = "Check if can exit specific vehicle"
            },
            new()
            {
                Conditional = "channeling:",
                ConfigurationKey = "channeling",
                Label = "Channeling Spell",
                Placeholder = "Spell name",
                Type = ConfigurationType.Text,
                Description = "Check if channeling specific spell"
            },
            new()
            {
                Conditional = "casting:",
                ConfigurationKey = "casting",
                Label = "Casting Spell",
                Placeholder = "Spell name",
                Type = ConfigurationType.Text,
                Description = "Check if casting specific spell"
            }
        };
    }

    private string FormatSingleConditionalWithConfiguration(string condition, Dictionary<string, string> configuration)
    {
        // Normalize: ensure trailing colon for advanced conditionals
        string normalized = condition.EndsWith(":") ? condition : condition + ":";

        // Handle special cases first
        if (normalized == "equipped:" && configuration.TryGetValue("equippedType", out var eqType))
        {
            if (eqType == "slot" && configuration.TryGetValue("equippedSlot", out var eqSlot) && !string.IsNullOrEmpty(eqSlot))
            {
                return FormatConditional("equipped", eqSlot);
            }

            if (eqType == "item" && configuration.TryGetValue("equippedItem", out var eqItem) && !string.IsNullOrEmpty(eqItem))
            {
                return FormatConditional("equipped", eqItem);
            }

            return "equipped:";
        }

        var configKey = GetConfigurationKeyForConditional(normalized);
        if (configKey != null && configuration.TryGetValue(configKey, out var value) && !string.IsNullOrEmpty(value))
        {
            var key = normalized.TrimEnd(':');
            return FormatConditional(key, value);
        }

        return condition;
    }

    public IEnumerable<string> GetAllConfigurationKeys()
    {
        return GetConditionalConfigurationInfo()
            .Select(info => info.ConfigurationKey)
            .Distinct();
    }
}
