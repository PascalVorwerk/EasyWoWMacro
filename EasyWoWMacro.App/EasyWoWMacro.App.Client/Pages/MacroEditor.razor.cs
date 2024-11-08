using EasyWoWMacro.Business;
using EasyWoWMacro.Business.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.App.Client.Pages;

public partial class MacroEditor : ComponentBase
{
    [Inject] public IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] public ILogger<MacroEditor> Logger { get; set; } = default!;
    
    private Macro CurrentMacro { get; set; } = new Macro();
    private CommandVerb? DraggedVerb { get; set; }

    private IJSObjectReference _module = default!;
    private string searchQuery = string.Empty;
    
    private CommandObject? selectedCommandObject;
    private Condition? selectedCondition;
    private OptionWord? selectedConditionPhrase;
    
    private Dictionary<string, List<CommandVerb>> FilteredCommandsByCategory =>
        MacroElements.AllCommands
            .Where(c => string.IsNullOrWhiteSpace(searchQuery) || 
                        c.DisplayName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || 
                        c.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            .GroupBy(c => GetCommandCategory(c))
            .ToDictionary(g => g.Key, g => g.ToList());

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load JavaScript to initialize event prevention on drop area
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/MacroEditor.razor.js");
            await _module.InvokeVoidAsync("initializeDropArea", "dropArea");
        }
    }
    
    // C# method to capture the item being dragged
    private void OnDragStart(CommandVerb verb)
    {
        DraggedVerb = verb;
        Logger.LogInformation($"Dragging verb: {verb.Name}");
    }
    
    // C# method to handle drop and add the dragged item to the list
    private void OnDrop()
    {
        if (DraggedVerb != null)
        {
            Logger.LogInformation($"Dropped a verb: {DraggedVerb.Name}");
            CurrentMacro.Commands.Add(new Command { CommandVerb = DraggedVerb });
            DraggedVerb = null; // Clear after drop
        }
        else
        {
            Logger.LogError("No verb was set for the drop event.");
        }
    }

    private void DeleteCommand(Command command)
    {
        CurrentMacro.Commands.Remove(command);
        Logger.LogInformation($"Deleted command: {command.CommandVerb?.Name}");
    }

    private void OnSearchInput(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? string.Empty;
        Logger.LogInformation($"Search query updated: {searchQuery}");
    }
    
    private string GetCommandCategory(CommandVerb command)
    {
        if (MacroElements.BattlePetCommands.Contains(command))
            return "Battle Pet Commands";
        if (MacroElements.InterfaceCommands.Contains(command))
            return "Interface Commands";
        if (MacroElements.ChatCommands.Contains(command))
            return "Chat Commands";
        if (MacroElements.CharacterCommands.Contains(command))
            return "Character Commands";
        if (MacroElements.DevToolsCommands.Contains(command))
            return "DevTools Commands";
        if (MacroElements.EmoteCommands.Contains(command))
            return "Emote Commands";
        
        return "Other Commands";
    }
    private void AddCommandObject(Command command)
    {
        var newCommandObject = new CommandObject();
        command.CommandObjects.Add(newCommandObject);
        Logger.LogInformation("CommandObject added.");
    }

    private void DeleteCommandObject(Command command, CommandObject commandObject)
    {
        command.CommandObjects.Remove(commandObject);
        Logger.LogInformation("CommandObject removed.");
    }

    // Adds a new Condition with a single ConditionPhrase to the CommandObject
    private void AddCondition(CommandObject commandObject)
    {
        var newCondition = new Condition();
        newCondition.ConditionPhrases.Add(new ConditionPhrase { Option = MacroConditions.TargetingConditions.Find(x => x.Value == "@target")});
        commandObject.Conditions.Add(newCondition);
        Logger.LogInformation("Condition added with a default ConditionPhrase.");
    }

    // Adds a new Parameter to the CommandObject
    private void AddParameter(CommandObject commandObject)
    {
        commandObject.Parameters.Add(new Parameter { Value = "BobbleBob" });
        Logger.LogInformation("Parameter added: ExampleParameter.");
    }

    private void RemoveCondition(CommandObject commandObject, Condition condition) => commandObject.Conditions.Remove(condition);

    private void RemoveParameter(CommandObject commandObject, Parameter parameter) => commandObject.Parameters.Remove(parameter);
    private void CopyToClipboard()
    {
        var macroText = CurrentMacro.ToString();
        // Call JavaScript to copy text to clipboard
        JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", macroText);
    }
}