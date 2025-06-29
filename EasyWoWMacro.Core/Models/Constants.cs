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
        "/cast", "/use", "/castsequence", "/stopcasting", "/stopmacro",
        
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
    /// All valid WoW macro conditionals organized by category
    /// </summary>
    public static class Conditionals
    {
        /// <summary>
        /// Modifier conditionals (require values: alt, ctrl, shift)
        /// </summary>
        public static readonly string[] Modifiers = 
        {
            "mod", "nomod", "modifier"
        };

        /// <summary>
        /// Valid values for modifier conditionals
        /// </summary>
        public static readonly string[] ModifierValues = 
        {
            "alt", "ctrl", "shift"
        };

        /// <summary>
        /// Target conditionals (no values required)
        /// </summary>
        public static readonly string[] Targets = 
        {
            "@target", "@mouseover", "@focus", "@player", "@cursor",
            "@arena1", "@arena2", "@arena3",
            "@party1", "@party2", "@party3", "@party4",
            "@raid1", "@raid2", "@raid3", "@raid4", "@raid5", "@raid6", "@raid7", "@raid8",
            "@raid9", "@raid10", "@raid11", "@raid12", "@raid13", "@raid14", "@raid15",
            "@raid16", "@raid17", "@raid18", "@raid19", "@raid20", "@raid21", "@raid22",
            "@raid23", "@raid24", "@raid25", "@raid26", "@raid27", "@raid28", "@raid29",
            "@raid30", "@raid31", "@raid32", "@raid33", "@raid34", "@raid35", "@raid36",
            "@raid37", "@raid38", "@raid39", "@raid40"
        };

        /// <summary>
        /// Combat and state conditionals (no values required)
        /// </summary>
        public static readonly string[] CombatAndState = 
        {
            "combat", "nocombat", "harm", "help", "dead", "nodead", "stealth", "nostealth",
            "mounted", "nomounted", "flying", "noflying", "swimming", "noswimming",
            "indoors", "outdoors", "group", "raid", "party", "solo", "pet", "nopet",
            "channeling", "nocasting", "casting", "exists", "noexists", "threat"
        };

        /// <summary>
        /// Form conditionals (require values: bear, cat, travel, aquatic, flight, moonkin, tree, battle, defensive, berserker)
        /// </summary>
        public static readonly string[] Forms = 
        {
            "form"
        };

        /// <summary>
        /// Valid values for form conditionals
        /// </summary>
        public static readonly string[] FormValues = 
        {
            "bear", "cat", "travel", "aquatic", "flight", "moonkin", "tree", "battle", "defensive", "berserker"
        };

        /// <summary>
        /// Stance conditionals (require values: 1, 2, 3, 4, 5, 6)
        /// </summary>
        public static readonly string[] Stances = 
        {
            "stance"
        };

        /// <summary>
        /// Valid values for stance conditionals
        /// </summary>
        public static readonly string[] StanceValues = 
        {
            "1", "2", "3", "4", "5", "6"
        };

        /// <summary>
        /// Equipment conditionals (require values: weapon, offhand, shield, 2h, 1h, ranged, ammo)
        /// </summary>
        public static readonly string[] Equipment = 
        {
            "equipped", "worn"
        };

        /// <summary>
        /// Valid values for equipment conditionals
        /// </summary>
        public static readonly string[] EquipmentValues = 
        {
            "weapon", "offhand", "shield", "2h", "1h", "ranged", "ammo"
        };

        /// <summary>
        /// Action bar conditionals (require values: 1, 2, 3, 4, 5, 6)
        /// </summary>
        public static readonly string[] ActionBars = 
        {
            "actionbar", "bar"
        };

        /// <summary>
        /// Valid values for action bar conditionals
        /// </summary>
        public static readonly string[] ActionBarValues = 
        {
            "1", "2", "3", "4", "5", "6"
        };

        /// <summary>
        /// Button conditionals (require values: 1, 2, 3, 4, 5)
        /// </summary>
        public static readonly string[] Buttons = 
        {
            "button"
        };

        /// <summary>
        /// Valid values for button conditionals
        /// </summary>
        public static readonly string[] ButtonValues = 
        {
            "1", "2", "3", "4", "5"
        };

        /// <summary>
        /// Threat conditionals (require values: 1, 2, 3)
        /// </summary>
        public static readonly string[] Threat = 
        {
            "threat"
        };

        /// <summary>
        /// Valid values for threat conditionals
        /// </summary>
        public static readonly string[] ThreatValues = 
        {
            "1", "2", "3"
        };

        /// <summary>
        /// UI state conditionals (no values required)
        /// </summary>
        public static readonly string[] UIState = 
        {
            "extrabar", "noextrabar", "possessbar", "nopossessbar", "overridebar", "nooverridebar",
            "vehicleui", "novehicleui", "unithasvehicleui", "nounithasvehicleui",
            "canexitvehicle", "nocanexitvehicle", "invehicle", "notinvehicle"
        };

        /// <summary>
        /// Target conditionals without @ prefix (no values required)
        /// </summary>
        public static readonly string[] TargetUnits = 
        {
            "player", "target", "mouseover", "focus", "cursor",
            "arena1", "arena2", "arena3",
            "party1", "party2", "party3", "party4",
            "raid1", "raid2", "raid3", "raid4", "raid5", "raid6", "raid7", "raid8",
            "raid9", "raid10", "raid11", "raid12", "raid13", "raid14", "raid15",
            "raid16", "raid17", "raid18", "raid19", "raid20", "raid21", "raid22",
            "raid23", "raid24", "raid25", "raid26", "raid27", "raid28", "raid29",
            "raid30", "raid31", "raid32", "raid33", "raid34", "raid35", "raid36",
            "raid37", "raid38", "raid39", "raid40"
        };

        /// <summary>
        /// Get all conditionals that require values
        /// </summary>
        public static readonly string[] ConditionalsWithValues = 
        {
            "mod", "nomod", "modifier", "form", "stance", "equipped", "worn", "actionbar", "bar", "button", "threat"
        };

        /// <summary>
        /// Get all conditionals that don't require values
        /// </summary>
        public static readonly string[] ConditionalsWithoutValues = 
        {
            "@target", "@mouseover", "@focus", "@player", "@cursor",
            "@arena1", "@arena2", "@arena3", "@party1", "@party2", "@party3", "@party4",
            "@raid1", "@raid2", "@raid3", "@raid4", "@raid5", "@raid6", "@raid7", "@raid8",
            "@raid9", "@raid10", "@raid11", "@raid12", "@raid13", "@raid14", "@raid15",
            "@raid16", "@raid17", "@raid18", "@raid19", "@raid20", "@raid21", "@raid22",
            "@raid23", "@raid24", "@raid25", "@raid26", "@raid27", "@raid28", "@raid29",
            "@raid30", "@raid31", "@raid32", "@raid33", "@raid34", "@raid35", "@raid36",
            "@raid37", "@raid38", "@raid39", "@raid40",
            "combat", "nocombat", "harm", "help", "dead", "nodead", "stealth", "nostealth",
            "mounted", "nomounted", "flying", "noflying", "swimming", "noswimming",
            "indoors", "outdoors", "group", "raid", "party", "solo", "pet", "nopet",
            "channeling", "nocasting", "casting", "exists", "noexists",
            "extrabar", "noextrabar", "possessbar", "nopossessbar", "overridebar", "nooverridebar",
            "vehicleui", "novehicleui", "unithasvehicleui", "nounithasvehicleui",
            "canexitvehicle", "nocanexitvehicle", "invehicle", "notinvehicle",
            "player", "target", "mouseover", "focus", "cursor",
            "arena1", "arena2", "arena3", "party1", "party2", "party3", "party4",
            "raid1", "raid2", "raid3", "raid4", "raid5", "raid6", "raid7", "raid8",
            "raid9", "raid10", "raid11", "raid12", "raid13", "raid14", "raid15",
            "raid16", "raid17", "raid18", "raid19", "raid20", "raid21", "raid22",
            "raid23", "raid24", "raid25", "raid26", "raid27", "raid28", "raid29",
            "raid30", "raid31", "raid32", "raid33", "raid34", "raid35", "raid36",
            "raid37", "raid38", "raid39", "raid40"
        };

        /// <summary>
        /// Get all conditionals (both with and without values)
        /// </summary>
        public static readonly string[] AllConditionals = 
        {
            // Modifiers
            "mod", "nomod", "modifier",
            // Targets with @
            "@target", "@mouseover", "@focus", "@player", "@cursor",
            "@arena1", "@arena2", "@arena3", "@party1", "@party2", "@party3", "@party4",
            "@raid1", "@raid2", "@raid3", "@raid4", "@raid5", "@raid6", "@raid7", "@raid8",
            "@raid9", "@raid10", "@raid11", "@raid12", "@raid13", "@raid14", "@raid15",
            "@raid16", "@raid17", "@raid18", "@raid19", "@raid20", "@raid21", "@raid22",
            "@raid23", "@raid24", "@raid25", "@raid26", "@raid27", "@raid28", "@raid29",
            "@raid30", "@raid31", "@raid32", "@raid33", "@raid34", "@raid35", "@raid36",
            "@raid37", "@raid38", "@raid39", "@raid40",
            // Combat and state
            "combat", "nocombat", "harm", "help", "dead", "nodead", "stealth", "nostealth",
            "mounted", "nomounted", "flying", "noflying", "swimming", "noswimming",
            "indoors", "outdoors", "group", "raid", "party", "solo", "pet", "nopet",
            "channeling", "nocasting", "casting", "exists", "noexists", "threat",
            // Forms and stances
            "form", "stance",
            // Equipment
            "equipped", "worn",
            // Action bars
            "actionbar", "bar",
            // Buttons
            "button",
            // UI state
            "extrabar", "noextrabar", "possessbar", "nopossessbar", "overridebar", "nooverridebar",
            "vehicleui", "novehicleui", "unithasvehicleui", "nounithasvehicleui",
            "canexitvehicle", "nocanexitvehicle", "invehicle", "notinvehicle",
            // Target units without @
            "player", "target", "mouseover", "focus", "cursor",
            "arena1", "arena2", "arena3", "party1", "party2", "party3", "party4",
            "raid1", "raid2", "raid3", "raid4", "raid5", "raid6", "raid7", "raid8",
            "raid9", "raid10", "raid11", "raid12", "raid13", "raid14", "raid15",
            "raid16", "raid17", "raid18", "raid19", "raid20", "raid21", "raid22",
            "raid23", "raid24", "raid25", "raid26", "raid27", "raid28", "raid29",
            "raid30", "raid31", "raid32", "raid33", "raid34", "raid35", "raid36",
            "raid37", "raid38", "raid39", "raid40"
        };

        /// <summary>
        /// Get valid values for a specific conditional
        /// </summary>
        /// <param name="conditional">The conditional key</param>
        /// <returns>Array of valid values for the conditional</returns>
        public static string[] GetValidValues(string conditional)
        {
            return conditional switch
            {
                "mod" or "nomod" or "modifier" => ModifierValues,
                "form" => FormValues,
                "stance" => StanceValues,
                "equipped" or "worn" => EquipmentValues,
                "actionbar" or "bar" => ActionBarValues,
                "button" => ButtonValues,
                "threat" => ThreatValues,
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// Check if a conditional requires a value
        /// </summary>
        /// <param name="conditional">The conditional key</param>
        /// <returns>True if the conditional requires a value</returns>
        public static bool RequiresValue(string conditional)
        {
            return ConditionalsWithValues.Contains(conditional);
        }
    }

    /// <summary>
    /// Conditional prefixes that indicate a conditional with a value
    /// </summary>
    public static readonly string[] ConditionalPrefixes = 
    {
        "mod:", "nomod:", "button:", "form:", "stance:", "equipped:", "worn:", "threat:"
    };
} 