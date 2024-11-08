using EasyWoWMacro.Business.Models;

namespace EasyWoWMacro.Business;

public static class MacroConditions
{
    // --- Targeting Conditions ---
    public static readonly List<OptionWord> TargetingConditions = new()
    {
        new OptionWord("Target Player", "@player", "Targets yourself"),
        new OptionWord("Target Focus", "@focus", "Targets your focus"),
        new OptionWord("Target Target", "@target", "Targets your selected target"),
        new OptionWord("Target Mouseover", "@mouseover", "Targets the unit you are hovering over"),
        new OptionWord("Target Cursor", "@cursor", "Targets the ground under the cursor"),
        new OptionWord("No Target", "@none", "Requires a targeting cursor, overrides auto self-cast")
    };

    // --- Boolean Conditions ---
    public static readonly List<OptionWord> BooleanConditions = new()
    {
        new OptionWord("Combat", "combat", "True if you are in combat"),
        new OptionWord("No Combat", "nocombat", "True if you are not in combat"),
        new OptionWord("Exists", "exists", "True if the target exists"),
        new OptionWord("No Exists", "noexists", "True if the target does not exist"),
        new OptionWord("Dead", "dead", "True if the target is dead"),
        new OptionWord("Alive", "nodead", "True if the target is alive"),
        new OptionWord("Friendly", "help", "True if the target is friendly"),
        new OptionWord("Hostile", "harm", "True if the target is hostile"),
        new OptionWord("Party Member", "party", "True if the target is in your party"),
        new OptionWord("Raid Member", "raid", "True if the target is in your raid"),
        new OptionWord("Grouped", "group", "True if you are in a party or raid"),
        new OptionWord("Not Grouped", "nogroup", "True if you are not in a party or raid"),
        new OptionWord("Indoors", "indoors", "True if you are indoors"),
        new OptionWord("Outdoors", "outdoors", "True if you are outdoors"),
        new OptionWord("Mounted", "mounted", "True if you are mounted"),
        new OptionWord("Not Mounted", "nomounted", "True if you are not mounted"),
        new OptionWord("Stealthed", "stealth", "True if you are in stealth"),
        new OptionWord("Not Stealthed", "nostealth", "True if you are not in stealth"),
        new OptionWord("Flying", "flying", "True if you are flying"),
        new OptionWord("Not Flying", "noflying", "True if you are not flying"),
        new OptionWord("Swimming", "swimming", "True if you are swimming"),
        new OptionWord("Not Swimming", "noswimming", "True if you are not swimming"),
        new OptionWord("Pet Active", "pet", "True if you have a pet active"),
        new OptionWord("No Pet Active", "nopet", "True if you do not have a pet active"),
        new OptionWord("Channeling", "channeling", "True if you are channeling a spell"),
        new OptionWord("Not Channeling", "nochanneling", "True if you are not channeling a spell"),
        new OptionWord("Modifier Key Pressed", "modifier", "True if a modifier key (Alt, Ctrl, Shift) is pressed"),
        new OptionWord("No Modifier Key Pressed", "nomodifier", "True if no modifier key is pressed"),
        new OptionWord("Shapeshift Form", "stance", "True if you are in a specific stance or form"),
        new OptionWord("No Shapeshift Form", "nostance", "True if you are not in a specific stance or form"),
        new OptionWord("Equipped Item", "equipped", "True if a specific item is equipped"),
        new OptionWord("Not Equipped Item", "noequipped", "True if a specific item is not equipped"),
        new OptionWord("Flyable Zone", "flyable", "True if you are in a zone where flying is possible"),
        new OptionWord("No Flyable Zone", "noflyable", "True if you are in a zone where flying is not possible"),
        new OptionWord("Vehicle UI Active", "vehicleui", "True if the vehicle UI is active"),
        new OptionWord("No Vehicle UI", "novehicleui", "True if the vehicle UI is not active"),
        new OptionWord("Pet Battle", "petbattle", "True if you are in a pet battle"),
        new OptionWord("No Pet Battle", "nopetbattle", "True if you are not in a pet battle"),
        new OptionWord("Specialization Active", "spec", "True if you are in a specific talent specialization"),
        new OptionWord("No Specialization", "nospec", "True if you are not in a specific talent specialization"),
        new OptionWord("Vehicle UI on Target", "unithasvehicleui", "True if the target has a vehicle UI"),
        new OptionWord("No Vehicle UI on Target", "nounithasvehicleui", "True if the target does not have a vehicle UI"),
        new OptionWord("Vehicle Active", "vehicle", "True if you are in a vehicle"),
        new OptionWord("No Vehicle Active", "novehicle", "True if you are not in a vehicle"),
        new OptionWord("Possess Bar Active", "possessbar", "True if the possess bar is active"),
        new OptionWord("No Possess Bar", "nopossessbar", "True if the possess bar is not active"),
        new OptionWord("Override Bar Active", "overridebar", "True if the override bar is active"),
        new OptionWord("No Override Bar", "nooverridebar", "True if the override bar is not active"),
        new OptionWord("Extra Bar Active", "extrabar", "True if the extra bar is active"),
        new OptionWord("No Extra Bar", "noextrabar", "True if the extra bar is not active"),
        new OptionWord("Pet Bar Active", "petbar", "True if the pet bar is active"),
        new OptionWord("No Pet Bar", "nopetbar", "True if the pet bar is not active"),
        new OptionWord("Shapeshift Active", "shapeshift", "True if you are in a shapeshift form"),
        new OptionWord("No Shapeshift", "noshapeshift", "True if you are not in a shapeshift form"),
        new OptionWord("Talent Specialization", "spec", "True if you are in a specific talent specialization"),
        new OptionWord("No Specialization", "nospec", "True if you are not in a specific specialization")
    };
}
