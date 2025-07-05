using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;

namespace EasyWoWMacro.Console;

sealed class Program
{
    static void Main(string[] args)
    {
        /*System.Console.WriteLine("=== EasyWoW Macro Testing Console ===");
        System.Console.WriteLine();

        // Test: Parse macro text with conditionals
        var parser = new MacroParser();
        var macroText = @"#showtooltip Fireball
/cast [mod:shift,@focus] Polymorph
/use [combat] Mana Potion; [nocombat] Water
; This is a comment";

        System.Console.WriteLine("Input macro text:");
        System.Console.WriteLine(macroText);
        System.Console.WriteLine();

        var macro = parser.Parse(macroText);
        System.Console.WriteLine("Parsed macro lines:");
        foreach (var line in macro.Lines)
        {
            System.Console.Write($"Line {line.LineNumber}: ");
            switch (line)
            {
                case DirectiveLine d:
                    System.Console.WriteLine($"Directive: {d.Directive} Argument: {d.Argument}");
                    break;
                case CommandLine c:
                    System.Console.Write($"Command: {c.Command}");
                    if (c.Clauses.Count > 0)
                    {
                        System.Console.Write(" Clauses:");
                        foreach (var clause in c.Clauses)
                        {
                            System.Console.Write($" [{clause}]");
                        }
                    }
                    System.Console.WriteLine();
                    break;
                case CommentLine cm:
                    System.Console.WriteLine($"Comment: {cm.Comment}");
                    break;
                default:
                    System.Console.WriteLine($"Unknown line type: {line.RawText}");
                    break;
            }
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Validation:");
        var errors = parser.ValidateMacro(macro);
        if (errors.Count == 0)
        {
            System.Console.WriteLine("✓ Macro is valid");
        }
        else
        {
            foreach (var error in errors)
            {
                System.Console.WriteLine($"✗ {error}");
            }
        }

        System.Console.WriteLine();
        System.Console.WriteLine(new string('=', 50));
        System.Console.WriteLine();

        // Test: Invalid macro with bad conditionals
        System.Console.WriteLine("Test: Invalid macro with bad conditionals");
        var invalidMacroText = @"#showtooltip
/cast [invalidcondition] Fireball
/use [mod:invalid] Mana Potion";

        System.Console.WriteLine("Input macro text:");
        System.Console.WriteLine(invalidMacroText);
        System.Console.WriteLine();

        var invalidMacro = parser.Parse(invalidMacroText);
        var invalidErrors = parser.ValidateMacro(invalidMacro);
        System.Console.WriteLine("Validation errors:");
        foreach (var error in invalidErrors)
        {
            System.Console.WriteLine($"✗ {error}");
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Press any key to exit...");
        System.Console.ReadKey();*/
    }
}
