using EasyWowMacro.Business.Models;

namespace EasyWoWMacro.Client.Data;

public class TargetConditionals
{
    public static List<Modifier> Conditionals = new()
    {
        new Modifier()
        {
            Name = "Exists",
            Value = "exists"
        },
        new Modifier()
        {
            Name = "Harm",
            Value = "harm"
        },
        new Modifier()
        {
            Name = "help",
            Value = "help"
        },
        new Modifier()
        {
            Name = "Dead",
            Value = "dead"
        },
        new Modifier()
        {
            Name = "Party",
            Value = "party"
        },
        new Modifier()
        {
            Name = "Raid",
            Value = "raid"
        },
        new Modifier()
        {
            Name = "Unit has vehicle UI",
            Value = "unithasvehicleui"
        }
    };
}