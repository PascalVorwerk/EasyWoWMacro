namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Validates WoW macro commands against known valid commands
/// </summary>
public static class CommandValidator
{
    /// <summary>
    /// Valid commands in WoW macros
    /// </summary>
    private static readonly HashSet<string> ValidCommands = new(StringComparer.OrdinalIgnoreCase)
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
    /// Validates a command against known valid commands
    /// </summary>
    /// <param name="command">The command to validate</param>
    /// <returns>True if the command is valid</returns>
    public static bool IsValidCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return false;

        // Extract the base command (before any arguments or conditionals)
        var baseCommand = ExtractBaseCommand(command);
        return ValidCommands.Contains(baseCommand);
    }

    /// <summary>
    /// Extracts the base command from a full command string
    /// </summary>
    /// <param name="command">The full command string</param>
    /// <returns>The base command without arguments or conditionals</returns>
    public static string ExtractBaseCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return string.Empty;

        // Find the first space or bracket to determine where the command ends
        var firstSpace = command.IndexOf(' ');
        var firstBracket = command.IndexOf('[');
        
        var endIndex = -1;
        if (firstSpace != -1 && firstBracket != -1)
        {
            endIndex = Math.Min(firstSpace, firstBracket);
        }
        else if (firstSpace != -1)
        {
            endIndex = firstSpace;
        }
        else if (firstBracket != -1)
        {
            endIndex = firstBracket;
        }

        return endIndex != -1 ? command.Substring(0, endIndex) : command;
    }

    /// <summary>
    /// Gets all valid commands
    /// </summary>
    /// <returns>List of valid commands</returns>
    public static IEnumerable<string> GetValidCommands()
    {
        return ValidCommands.OrderBy(c => c);
    }

    /// <summary>
    /// Validates a command line and returns any validation errors
    /// </summary>
    /// <param name="commandLine">The command line to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateCommandLine(CommandLine commandLine)
    {
        var errors = new List<string>();

        if (!IsValidCommand(commandLine.Command))
        {
            errors.Add($"Invalid command: {commandLine.Command}");
        }

        return errors;
    }
} 