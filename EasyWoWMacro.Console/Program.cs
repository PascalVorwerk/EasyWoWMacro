using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.Core.Services;

namespace EasyWoWMacro.Console;

sealed class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--count")
        {
            if (args.Length > 1)
            {
                System.Console.WriteLine(args[1].Length);
                return;
            }
        }

        if (args.Length > 0 && args[0] == "--debug")
        {
            var enhancer = new ValidationEnhancementService();
            var parser = new MacroParser();

            System.Console.WriteLine("=== Testing Character Limit ===");
            var macroText1 = "/cast [mod:shift,combat,harm,nodead,exists] Fireball; [mod:ctrl,combat,harm,nodead,exists] Frostbolt; [mod:alt,combat,harm,nodead,exists] Polymorph; [combat,harm,nodead,exists] Fireball; [@target,harm,nodead] Fireball";
            System.Console.WriteLine($"Macro length: {macroText1.Length}");
            var basicErrors1 = parser.ValidateMacroText(macroText1);
            System.Console.WriteLine($"Basic errors count: {basicErrors1.Count}");
            var enhancedErrors1 = enhancer.EnhanceValidationErrors(basicErrors1, macroText1);
            System.Console.WriteLine($"Enhanced errors count: {enhancedErrors1.Count}");
            foreach (var error in enhancedErrors1)
            {
                System.Console.WriteLine($"- {error.Message} (Type: {error.Type})");
            }

            System.Console.WriteLine("\n=== Testing Directive Placement ===");
            var macroText2 = @"/cast Fireball
#showtooltip Fireball";
            System.Console.WriteLine($"Macro text: '{macroText2}'");
            var basicErrors2 = parser.ValidateMacroText(macroText2);
            System.Console.WriteLine($"Basic errors count: {basicErrors2.Count}");
            var enhancedErrors2 = enhancer.EnhanceValidationErrors(basicErrors2, macroText2);
            System.Console.WriteLine($"Enhanced errors count: {enhancedErrors2.Count}");
            foreach (var error in enhancedErrors2)
            {
                System.Console.WriteLine($"- {error.Message} (Type: {error.Type})");
            }
            return;
        }

        System.Console.WriteLine("Use --debug to run validation debug tests");
    }
}
