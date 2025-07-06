using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Web.Client.Models;

namespace EasyWoWMacro.Web.Client.Services;

/// <summary>
/// Implementation of IConditionalService that uses core validation classes
/// </summary>
public class ConditionalService : IConditionalService
{

    // Advanced conditional prefixes that require configuration
    private static readonly HashSet<string> AdvancedConditionalPrefixes =
    [
        "mod:", "nomod:", "modifier:", "stance:", "nostance:", "equipped:", "worn:", "form:", "noform:", "button:", "btn:",
        "threat:", "spec:", "talent:", "glyph:", "spell:", "item:", "aura:", "buff:", "debuff:",
        "actionbar:", "bar:", "vehicle:", "canexitvehicle:", "channeling:", "casting:",
        "known:", "noknown:", "pet:", "bonusbar:"
    ];

    // Conditionals that always require values and should never appear as basic conditions
    private static readonly HashSet<string> AlwaysRequireValues =
    [
        "form", "noform", "stance", "nostance", "equipped", "worn", "actionbar", "bar", 
        "button", "btn", "threat", "spec", "talent", "glyph", "spell", "item", "aura", 
        "buff", "debuff", "known", "noknown", "pet"
    ];

    public IEnumerable<string> GetValidConditionalKeys()
    {
        return ConditionalValidator.GetValidConditionalKeys();
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
            .Where(key => !AdvancedConditionalPrefixes.Contains(key) && 
                         !key.EndsWith(":") && 
                         !AlwaysRequireValues.Contains(key))
            .OrderBy(k => k);
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
                Conditional = "modifier:",
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
                    new SelectOption { Value = "1", Label = "1 - Form/Stance 1" },
                    new SelectOption { Value = "2", Label = "2 - Form/Stance 2" },
                    new SelectOption { Value = "3", Label = "3 - Form/Stance 3" },
                    new SelectOption { Value = "4", Label = "4 - Form/Stance 4" },
                    new SelectOption { Value = "5", Label = "5 - Form/Stance 5" },
                    new SelectOption { Value = "6", Label = "6 - Form/Stance 6" }
                ]
            },
            new()
            {
                Conditional = "nostance:",
                ConfigurationKey = "stance",
                Label = "Not In Stance/Form",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Not Form/Stance 1" },
                    new SelectOption { Value = "2", Label = "2 - Not Form/Stance 2" },
                    new SelectOption { Value = "3", Label = "3 - Not Form/Stance 3" },
                    new SelectOption { Value = "4", Label = "4 - Not Form/Stance 4" },
                    new SelectOption { Value = "5", Label = "5 - Not Form/Stance 5" },
                    new SelectOption { Value = "6", Label = "6 - Not Form/Stance 6" }
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
                Label = "Form Number",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "0", Label = "0 - Humanoid Form" },
                    new SelectOption { Value = "1", Label = "1 - Form 1" },
                    new SelectOption { Value = "2", Label = "2 - Form 2" },
                    new SelectOption { Value = "3", Label = "3 - Form 3" },
                    new SelectOption { Value = "4", Label = "4 - Form 4" },
                    new SelectOption { Value = "5", Label = "5 - Form 5" },
                    new SelectOption { Value = "6", Label = "6 - Form 6" }
                ],
                Description = "Check current form by number (0=humanoid, class-specific forms 1-6)"
            },
            new()
            {
                Conditional = "noform:",
                ConfigurationKey = "form",
                Label = "Not In Form Number",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "0", Label = "0 - Not Humanoid Form" },
                    new SelectOption { Value = "1", Label = "1 - Not Form 1" },
                    new SelectOption { Value = "2", Label = "2 - Not Form 2" },
                    new SelectOption { Value = "3", Label = "3 - Not Form 3" },
                    new SelectOption { Value = "4", Label = "4 - Not Form 4" },
                    new SelectOption { Value = "5", Label = "5 - Not Form 5" },
                    new SelectOption { Value = "6", Label = "6 - Not Form 6" }
                ],
                Description = "Check player is not in specific form by number (0=not humanoid, etc.)"
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
            },
            new()
            {
                Conditional = "known:",
                ConfigurationKey = "known",
                Label = "Known Spell/Ability",
                Placeholder = "Spell name or ID",
                Type = ConfigurationType.Text,
                Description = "Check if player knows specific spell or ability"
            },
            new()
            {
                Conditional = "noknown:",
                ConfigurationKey = "known",
                Label = "Unknown Spell/Ability",
                Placeholder = "Spell name or ID",
                Type = ConfigurationType.Text,
                Description = "Check if player doesn't know specific spell or ability"
            },
            new()
            {
                Conditional = "pet:",
                ConfigurationKey = "pet",
                Label = "Pet Name/Family",
                Placeholder = "Pet name or family (e.g., 'Wolf', 'Fluffy')",
                Type = ConfigurationType.Text,
                Description = "Check pet by name or family type"
            },
            new()
            {
                Conditional = "bonusbar:",
                ConfigurationKey = "bonusbar",
                Label = "Bonus Bar Number",
                Type = ConfigurationType.Select,
                Options =
                [
                    new SelectOption { Value = "1", Label = "1 - Bonus Bar 1" },
                    new SelectOption { Value = "2", Label = "2 - Bonus Bar 2" },
                    new SelectOption { Value = "3", Label = "3 - Bonus Bar 3" },
                    new SelectOption { Value = "4", Label = "4 - Bonus Bar 4" },
                    new SelectOption { Value = "5", Label = "5 - Bonus Bar 5" }
                ],
                Description = "Check specific bonus action bar number"
            }
        };
    }
}
