namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Centralized constants for WoW macro validation and parsing
/// </summary>
public static class WoWMacroConstants
{
    /// <summary>
    /// Valid WoW macro directives
    /// </summary>
    public static readonly string[] ValidDirectives = 
    {
        "#showtooltip", "#show", "#hide", "#icon"
    };

    /// <summary>
    /// Valid WoW macro slash commands
    /// </summary>
    public static readonly string[] ValidSlashCommands = 
    {
        // Combat commands
        "/cast", "/use", "/castsequence", "/castrandom", "/stopcasting", "/stopmacro",
        
        // Target commands
        "/target", "/targetenemy", "/targetfriend", "/targetlasttarget", "/targetlastfriend",
        "/targetlastenemy", "/targetparty", "/targetraid", "/targetplayer", "/targetself",
        "/cleartarget", "/focus", "/clearfocus", "/assist", "/follow", "/dismount",
        
        // Pet commands
        "/petattack", "/petfollow", "/petstay", "/petpassive", "/petdefensive", "/petaggressive",
        "/petautocasttoggle", "/petautocaston", "/petautocastoff", "/petdismiss", "/petmove",
        
        // Equipment commands
        "/equip", "/equipslot", "/use", "/pickup", "/pickupcontaineritem",
        
        // Aura commands
        "/cancelaura", "/cancelform", "/cancelbuff", "/cancelqueuedspell",
        
        // Movement commands
        "/dismount", "/mount", "/sit", "/stand", "/sleep", "/dance", "/mountspecial",
        
        // Communication commands
        "/say", "/yell", "/whisper", "/party", "/raid", "/guild", "/officer", "/instance_chat",
        "/emote", "/me", "/chat", "/join", "/leave", "/invite", "/kick", "/promote", "/demote",
        
        // UI commands
        "/macro", "/script", "/run", "/dump", "/reload", "/logout", "/quit", "/exit",
        "/help", "/who", "/friend", "/ignore", "/block", "/unblock", "/report",
        "/showtooltip", "/show",
        
        // Combat log commands
        "/combatlog", "/log", "/clearlog",
        
        // Addon commands
        "/addon", "/addonlist", "/addonreload", "/addoncheck",
        
        // Utility commands
        "/random", "/roll", "/time", "/calendar", "/achievement", "/quest", "/questlog",
        "/spellbook", "/talent", "/specialization", "/profession", "/trade", "/craft",
        "/use", "/useitem", "/usecontaineritem", "/useinventoryitem",
        
        // Vehicle commands
        "/vehicle", "/vehicleexit", "/vehicleui",
        
        // Group commands
        "/readycheck", "/role", "/loot", "/lootmethod", "/lootthreshold",
        
        // PvP commands
        "/pvp", "/pvpflag", "/duel", "/forfeit", "/challenge", "/arena", "/battleground",
        
        // Instance commands
        "/instance", "/instance_reset", "/instance_lock", "/instance_unlock",
        
        // World commands
        "/world", "/worldmap", "/minimap", "/map", "/taxi", "/flightpath",
        
        // Character commands
        "/character", "/characterinfo", "/characterstats", "/characterreputation",
        "/characterachievements", "/charactertalents", "/characterspecialization",
        
        // Social commands
        "/friend", "/ignore", "/block", "/unblock", "/report", "/complain",
        "/whisper", "/tell", "/reply", "/reply2", "/reply3", "/reply4", "/reply5",
        
        // System commands
        "/system", "/systeminfo", "/systemsettings", "/systemoptions",
        "/graphics", "/sound", "/interface", "/accessibility",
        
        // Debug commands (limited set)
        "/dump", "/reload", "/script", "/run"
    };

    /// <summary>
    /// Known conditional keywords (without values) - Based on official WoW wiki
    /// </summary>
    public static readonly string[] KnownConditionals = 
    {
        // Combat and targeting
        "combat", "nocombat", "harm", "help", "dead", "nodead", "exists", "noexists",
        
        // Player state
        "stealth", "nostealth", "mounted", "nomounted", "flying", "noflying", 
        "swimming", "noswimming", "resting", "noresting", "petbattle", "nopetbattle",
        "pvpcombat", "nopvpcombat",
        
        // Environment
        "indoors", "outdoors", "flyable", "noflyable", "advflyable", "noadvflyable",
        
        // Group
        "group", "nogroup", "raid", "noraid", "party", "noparty",
        
        // Pet
        "pet", "nopet",
        
        // UI state
        "cursor", "nocursor", "extrabar", "noextrabar", "overridebar", "nooverridebar",
        "possessbar", "nopossessbar", "shapeshift", "noshapeshift", "bonusbar", "nobonusbar",
        "vehicleui", "novehicleui", "unithasvehicleui", "nounithasvehicleui",
        "canexitvehicle", "nocanexitvehicle"
    };

    /// <summary>
    /// Conditional prefixes that indicate a conditional with a value - Based on official WoW wiki
    /// </summary>
    public static readonly string[] ConditionalPrefixes = 
    {
        // Modifier keys
        "mod:", "nomod:", "modifier:",
        
        // Mouse buttons
        "button:", "btn:",
        
        // Forms and stances
        "form:", "stance:",
        
        // Equipment
        "equipped:", "worn:",
        
        // Action bars
        "actionbar:", "bar:", "bonusbar:",
        
        // Specialization and talents
        "spec:", "talent:",
        
        // Pet
        "pet:",
        
        // Spells
        "channeling:", "casting:", "known:", "unknown:",
        
        // Group
        "group:", "raid:",
        
        // Targeting
        "target:", "@"
    };
} 