namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Validates WoW macro conditionals against known valid conditionals
/// </summary>
public static class ConditionalValidator
{
    /// <summary>
    /// Valid conditionals in WoW macros
    /// </summary>
    private static readonly Dictionary<string, string[]> ValidConditionals = new()
    {
        // Modifier keys
        ["mod"] = ["alt", "ctrl", "shift"],
        ["nomod"] = ["alt", "ctrl", "shift"],
        
        // Target conditions
        ["@target"] = [],
        ["@mouseover"] = [],
        ["@focus"] = [],
        ["@player"] = [],
        ["@cursor"] = [],
        ["@arena1"] = [],
        ["@arena2"] = [],
        ["@arena3"] = [],
        ["@party1"] = [],
        ["@party2"] = [],
        ["@party3"] = [],
        ["@party4"] = [],
        ["@raid1"] = [],
        ["@raid2"] = [],
        ["@raid3"] = [],
        ["@raid4"] = [],
        ["@raid5"] = [],
        ["@raid6"] = [],
        ["@raid7"] = [],
        ["@raid8"] = [],
        ["@raid9"] = [],
        ["@raid10"] = [],
        ["@raid11"] = [],
        ["@raid12"] = [],
        ["@raid13"] = [],
        ["@raid14"] = [],
        ["@raid15"] = [],
        ["@raid16"] = [],
        ["@raid17"] = [],
        ["@raid18"] = [],
        ["@raid19"] = [],
        ["@raid20"] = [],
        ["@raid21"] = [],
        ["@raid22"] = [],
        ["@raid23"] = [],
        ["@raid24"] = [],
        ["@raid25"] = [],
        ["@raid26"] = [],
        ["@raid27"] = [],
        ["@raid28"] = [],
        ["@raid29"] = [],
        ["@raid30"] = [],
        ["@raid31"] = [],
        ["@raid32"] = [],
        ["@raid33"] = [],
        ["@raid34"] = [],
        ["@raid35"] = [],
        ["@raid36"] = [],
        ["@raid37"] = [],
        ["@raid38"] = [],
        ["@raid39"] = [],
        ["@raid40"] = [],
        
        // Combat conditions
        ["combat"] = [],
        ["nocombat"] = [],
        ["harm"] = [],
        ["help"] = [],
        ["dead"] = [],
        ["nodead"] = [],
        ["stealth"] = [],
        ["nostealth"] = [],
        ["mounted"] = [],
        ["nomounted"] = [],
        ["flying"] = [],
        ["noflying"] = [],
        ["swimming"] = [],
        ["noswimming"] = [],
        ["indoors"] = [],
        ["outdoors"] = [],
        ["group"] = [],
        ["raid"] = [],
        ["party"] = [],
        ["solo"] = [],
        ["pet"] = [],
        ["nopet"] = [],
        ["form"] = ["bear", "cat", "travel", "aquatic", "flight", "moonkin", "tree", "battle", "defensive", "berserker"],
        ["stance"] = ["1", "2", "3", "4", "5", "6"],
        ["equipped"] = ["weapon", "offhand", "shield", "2h", "1h", "ranged", "ammo"],
        ["worn"] = ["weapon", "offhand", "shield", "2h", "1h", "ranged", "ammo"],
        ["channeling"] = [],
        ["nocasting"] = [],
        ["casting"] = [],
        ["actionbar"] = ["1", "2", "3", "4", "5", "6"],
        ["bar"] = ["1", "2", "3", "4", "5", "6"],
        ["extrabar"] = [],
        ["noextrabar"] = [],
        ["possessbar"] = [],
        ["nopossessbar"] = [],
        ["overridebar"] = [],
        ["nooverridebar"] = [],
        ["vehicleui"] = [],
        ["novehicleui"] = [],
        ["unithasvehicleui"] = [],
        ["nounithasvehicleui"] = [],
        ["canexitvehicle"] = [],
        ["nocanexitvehicle"] = [],
        ["invehicle"] = [],
        ["notinvehicle"] = [],
        ["modifier"] = ["alt", "ctrl", "shift"],
        ["button"] = ["1", "2", "3", "4", "5"],
        ["exists"] = [],
        ["noexists"] = [],
        ["dead"] = [],
        ["nodead"] = [],
        ["threat"] = ["1", "2", "3"],
        ["harm"] = [],
        ["help"] = [],
        ["player"] = [],
        ["target"] = [],
        ["mouseover"] = [],
        ["focus"] = [],
        ["cursor"] = [],
        ["arena1"] = [],
        ["arena2"] = [],
        ["arena3"] = [],
        ["party1"] = [],
        ["party2"] = [],
        ["party3"] = [],
        ["party4"] = [],
        ["raid1"] = [],
        ["raid2"] = [],
        ["raid3"] = [],
        ["raid4"] = [],
        ["raid5"] = [],
        ["raid6"] = [],
        ["raid7"] = [],
        ["raid8"] = [],
        ["raid9"] = [],
        ["raid10"] = [],
        ["raid11"] = [],
        ["raid12"] = [],
        ["raid13"] = [],
        ["raid14"] = [],
        ["raid15"] = [],
        ["raid16"] = [],
        ["raid17"] = [],
        ["raid18"] = [],
        ["raid19"] = [],
        ["raid20"] = [],
        ["raid21"] = [],
        ["raid22"] = [],
        ["raid23"] = [],
        ["raid24"] = [],
        ["raid25"] = [],
        ["raid26"] = [],
        ["raid27"] = [],
        ["raid28"] = [],
        ["raid29"] = [],
        ["raid30"] = [],
        ["raid31"] = [],
        ["raid32"] = [],
        ["raid33"] = [],
        ["raid34"] = [],
        ["raid35"] = [],
        ["raid36"] = [],
        ["raid37"] = [],
        ["raid38"] = [],
        ["raid39"] = [],
        ["raid40"] = [],
    };

    /// <summary>
    /// Validates a condition against known valid conditionals
    /// </summary>
    /// <param name="condition">The condition to validate</param>
    /// <returns>True if the condition is valid</returns>
    public static bool IsValidCondition(Condition condition)
    {
        if (string.IsNullOrWhiteSpace(condition.Key))
            return false;

        // Check if the key exists
        if (!ValidConditionals.TryGetValue(condition.Key, out var validValues))
            return false;

        // If the condition has a value, check if it's valid for that key
        if (!string.IsNullOrWhiteSpace(condition.Value))
        {
            return validValues.Length == 0 || validValues.Contains(condition.Value);
        }

        return true;
    }

    /// <summary>
    /// Validates a condition set
    /// </summary>
    /// <param name="conditionSet">The condition set to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateConditionSet(ConditionSet conditionSet)
    {
        var errors = new List<string>();

        foreach (var condition in conditionSet.Conditions)
        {
            if (!IsValidCondition(condition))
            {
                errors.Add($"Invalid condition: {condition}");
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates a conditional block
    /// </summary>
    /// <param name="conditional">The conditional to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateConditional(Conditional conditional)
    {
        var errors = new List<string>();

        foreach (var conditionSet in conditional.ConditionSets)
        {
            errors.AddRange(ValidateConditionSet(conditionSet));
        }

        return errors;
    }

    /// <summary>
    /// Gets all valid conditional keys
    /// </summary>
    /// <returns>List of valid conditional keys</returns>
    public static IEnumerable<string> GetValidConditionalKeys()
    {
        return ValidConditionals.Keys;
    }

    /// <summary>
    /// Gets valid values for a specific conditional key
    /// </summary>
    /// <param name="key">The conditional key</param>
    /// <returns>List of valid values for the key</returns>
    public static IEnumerable<string> GetValidValuesForKey(string key)
    {
        return ValidConditionals.TryGetValue(key, out var values) ? values : Enumerable.Empty<string>();
    }
} 