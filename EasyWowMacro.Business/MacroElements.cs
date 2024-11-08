using EasyWoWMacro.Business.Models;

namespace EasyWoWMacro.Business;

public static class MacroElements
{
    // TODO: Update list to list of all macro commands: https://wowpedia.fandom.com/wiki/Macro_commands#Targeting_functions
    // --- Battle Pet Commands ---
    public static readonly List<CommandVerb> BattlePetCommands = new()
    {
        new CommandVerb("Random Favorite Pet", "randomfavoritepet", "Summon a random favorite battle pet."),
        new CommandVerb("Summon Pet", "summonpet", "Summon the specified battle pet."),
        new CommandVerb("Dismiss Pet", "dismisspet", "Dismiss your battle pet.")
    };

    // --- Blizzard Interface Commands ---
    public static readonly List<CommandVerb> InterfaceCommands = new()
    {
        new CommandVerb("Achievements", "achievements", "Opens the Achievements interface."),
        new CommandVerb("Calendar", "calendar", "Opens the Calendar interface."),
        new CommandVerb("Guild Finder", "guildfinder", "Opens the Guild Finder tool."),
        new CommandVerb("Dungeon Finder", "dungeonfinder", "Opens the Dungeon Finder interface."),
        new CommandVerb("Loot", "loot", "Opens loot history."),
        new CommandVerb("Macro", "macro", "Opens the Macro interface."),
        new CommandVerb("Raid Finder", "raidfinder", "Opens the Raid Browser."),
        new CommandVerb("Share", "share", "Share to Twitter."),
        new CommandVerb("Stopwatch", "stopwatch", "Opens the Stopwatch interface.")
    };

    // --- Chat Commands ---
    public static readonly List<CommandVerb> ChatCommands = new()
    {
        new CommandVerb("AFK", "afk", "Marks you as 'Away From Keyboard'."),
        new CommandVerb("Announce", "announce", "Toggle channel announcements."),
        new CommandVerb("Ban", "ban", "Bans a user from a user-created chat channel."),
        new CommandVerb("Battleground", "battleground", "Sends a chat message to your battleground."),
        new CommandVerb("Channel Say", "csay", "Sends chat text to a channel referenced by number only."),
        new CommandVerb("Chat Invite", "chatinvite", "Invites a user to a user-created chat channel."),
        new CommandVerb("Chat List", "chatlist", "Lists users in a user-created chat channel."),
        new CommandVerb("Chat Who", "chatwho", "Lists users in a user-created chat channel."),
        new CommandVerb("Channel Invite", "cinvite", "Invite a user to a user-created chat channel."),
        new CommandVerb("Channel Kick", "ckick", "Kicks a user from a user-created chat channel."),
        new CommandVerb("Combat Log", "combatlog", "Opens the combat log."),
        new CommandVerb("Console", "console", "Opens the console."),
        new CommandVerb("Do Not Disturb", "dnd", "Marks you as 'Do Not Disturb'."),
        new CommandVerb("Emote", "emote", "Performs an emote."),
        new CommandVerb("Ignore", "ignore", "Ignores a user."),
        new CommandVerb("Invite", "invite", "Invites a player to your group."),
        new CommandVerb("Join", "join", "Joins a chat channel."),
        new CommandVerb("Leave", "leave", "Leaves a chat channel."),
        new CommandVerb("Logout", "logout", "Logs you out of the game."),
        new CommandVerb("Macro Help", "macrohelp", "Displays help for macros."),
        new CommandVerb("Party", "party", "Sends a chat message to your party."),
        new CommandVerb("Raid", "raid", "Sends a chat message to your raid."),
        new CommandVerb("Reply", "reply", "Replies to the last whisper."),
        new CommandVerb("Say", "say", "Sends a chat message to nearby players."),
        new CommandVerb("Whisper", "whisper", "Sends a private message to a player."),
        new CommandVerb("Yell", "yell", "Sends a chat message to a wide area.")
    };

    // --- Character Commands ---
    public static readonly List<CommandVerb> CharacterCommands = new()
    {
        new CommandVerb("Dismount", "dismount", "Dismounts you from your current mount."),
        new CommandVerb("Equip", "equip", "Equips an item."),
        new CommandVerb("Equip Set", "equipset", "Equips a set of items."),
        new CommandVerb("Start Attack", "startattack", "Starts auto-attacking the target."),
        new CommandVerb("Stop Attack", "stopattack", "Stops auto-attacking."),
        new CommandVerb("Target", "target", "Targets a unit."),
        new CommandVerb("Target Enemy", "targetenemy", "Targets the nearest enemy."),
        new CommandVerb("Target Friend", "targetfriend", "Targets the nearest friendly unit."),
        new CommandVerb("Target Last Enemy", "targetlastenemy", "Targets the last enemy you targeted."),
        new CommandVerb("Target Last Friend", "targetlastfriend", "Targets the last friendly unit you targeted."),
        new CommandVerb("Target Party", "targetparty", "Targets a party member."),
        new CommandVerb("Target Raid", "targetraid", "Targets a raid member."),
        new CommandVerb("Use", "use", "Uses an item."),
        new CommandVerb("Use Random", "userandom", "Uses a random item from a list.")
    };

    // --- DevTools Commands ---
    public static readonly List<CommandVerb> DevToolsCommands = new()
    {
        new CommandVerb("Frame Stack", "framestack", "Opens the framestack tool."),
        new CommandVerb("Reload", "reload", "Reloads the user interface."),
        new CommandVerb("Script", "script", "Runs a Lua script.")
    };

    // --- Emote Commands ---
    public static readonly List<CommandVerb> EmoteCommands = new()
    {
        new CommandVerb("Dance", "dance", "Makes your character dance."),
        new CommandVerb("Laugh", "laugh", "Makes your character laugh."),
        new CommandVerb("Cry", "cry", "Makes your character cry."),
        new CommandVerb("Cheer", "cheer", "Makes your character cheer."),
        new CommandVerb("Wave", "wave", "Makes your character wave."),
        new CommandVerb("Bow", "bow", "Makes your character bow."),
        new CommandVerb("Kiss", "kiss", "Makes your character blow a kiss."),
        new CommandVerb("Hug", "hug", "Makes your character hug the target."),
        new CommandVerb("Pat", "pat", "Makes your character pat the target."),
        new CommandVerb("Salute", "salute", "Makes your character salute."),
        new CommandVerb("Thank", "thank", "Makes your character thank the target.")
    };
    
    // --- All Commands ---
    public static List<CommandVerb> AllCommands =>
        BattlePetCommands
            .Concat(InterfaceCommands)
            .Concat(ChatCommands)
            .Concat(CharacterCommands)
            .Concat(DevToolsCommands)
            .Concat(EmoteCommands)
            .ToList();
}

