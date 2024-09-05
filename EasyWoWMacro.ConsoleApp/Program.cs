// See https://aka.ms/new-console-template for more information

using EasyWowMacro.Business.Models;

var macro = new Macro
{
    Name = "TestMacro",
    Commands = new List<Command>
    {
        new Command()
        {
            Name = "Show tooltip",
            CommandType = "#showtooltip",
        },
        new Command
        {
            Name = "Cast spell, item, bag id slot or inventory id slot",
            CommandType = "/cast",
            Arguments = new List<Argument>
            {
                new Argument
                {
                    ArgumentValue = "Spell1",
                    ModifierGroups = new List<ModifierGroup>
                    {
                        new ModifierGroup
                        {
                            Modifiers = new List<Modifier>
                            {
                                new Modifier { Name = "@player", IsTargetModifier = true },
                                new Modifier { Name = "exists" }
                            }
                        },
                        new ModifierGroup
                        {
                            Modifiers = new List<Modifier>
                            {
                            }
                        }
                    }
                },
                new Argument
                {
                    ArgumentValue = "Spell2",
                    ModifierGroups = new List<ModifierGroup>
                    {
                        new ModifierGroup
                        {
                            Modifiers = new List<Modifier>
                            {
                                new Modifier { Name = "@player", IsTargetModifier = true },
                                new Modifier { Name = "exists" }
                            }
                        },
                        new ModifierGroup
                        {
                            Modifiers = new List<Modifier>
                            {
                            }
                        }
                    }
                },
            }
        }
    }
};

Console.WriteLine(macro);