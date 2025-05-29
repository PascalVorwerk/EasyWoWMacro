using EasyWoWMacro.Business.Models;

namespace EasyWoWMacro.Business;

public static class MacroItems
{
    public static class Commands
    {
        public static Command[]? All => Combat.Concat(Guild) as Command[];
        
        public static SecureCommand[] Combat =>
        [
            new("cancelaura", "Cancel Aura", "Cancels (turns off) an aura you have."),
            new ("cancelqueuedspell", "Cancel Queued Spell", "Cancels casting of the spell you have in the queue."),
            new ("cancelform", "Cancel Form", "Cancels your current shapeshift form."),
            new ("cast", "Cast", "Uses the stated spell or item."),
        ];

        public static SecureCommand[] Guild =>
        [
        ];
    }
}