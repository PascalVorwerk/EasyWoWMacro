using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EasyWoWMacro.Web.Models;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Components.BuildingBlocks;

public partial class CommandConfigurationModal : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public MacroBlockModel? Block { get; set; }

    [Parameter]
    public EventCallback<MacroBlockModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    private string selectedCommand = "";
    private string searchTerm = "";

    private IEnumerable<string> filteredCommands => 
        string.IsNullOrWhiteSpace(searchTerm) 
            ? WoWMacroConstants.ValidSlashCommands 
            : WoWMacroConstants.ValidSlashCommands.Where(cmd => 
                cmd.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("command", out var cmd))
        {
            selectedCommand = cmd;
        }
    }

    private void HandleSearchInput(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString() ?? "";
        StateHasChanged();
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(selectedCommand))
        {
            Block.Configuration["command"] = selectedCommand;
            Block.DisplayText = selectedCommand;
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 