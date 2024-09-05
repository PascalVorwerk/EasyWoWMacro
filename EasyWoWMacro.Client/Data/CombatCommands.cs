using EasyWowMacro.Business.Models;

namespace EasyWoWMacro.Client.Data;

public class CombatCommands
{
    public static List<Command> Commands = new()
    {
        new ()
        {
            Name = "Show tooltip",
            CommandType = "#showtooltip",
        },
        new()
        {
            Name = "Cancel queued spell",
            CommandType = "/cancelqueuedspell",
        },
        new()
        {
            Name = "Cancel form",
            CommandType = "/cancelaura",
        },
        new Command()
        {
            Name = "Cast spell, item, bag id slot or inventory id slot",
            CommandType = "/cast",
        },
        new Command()
        {
            Name = "Cast random (separator: ,)",
            CommandType = "/castrandom",
        },
        new Command()
        {
            Name = "Cast sequence",
            CommandType = "/castsequence",
        },
        new Command()
        {
            Name = "Change action bar",
            CommandType = "/changeactionbar",
        },
        new Command()
        {
            Name = "Start attack",
            CommandType = "/startattack",
        },
        new Command()
        {
            Name = "Stop attack",
            CommandType = "/stopattack",
        },
        new Command()
        {
            Name = "Stop casting",
            CommandType = "/stopcasting",
        },
        new Command()
        {
            Name = "Stop spell target",
            CommandType = "/stopspelltarget",
        },
        new Command()
        {
            Name = "Swap action bar",
            CommandType = "/swapactionbar",
        },
        new Command()
        {
            Name = "Use item",
            CommandType = "/use",
        },
        new Command()
        {
            Name = "Use random item",
            CommandType = "/userandom",
        },
        new Command()
        {
            Name = "Use toy",
            CommandType = "/usetoy",
        }
    };
}