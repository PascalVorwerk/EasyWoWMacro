using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Web.Client.Models;

namespace EasyWoWMacro.Web.Client.Components.BuildingBlocks;

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

    private string _selectedCommand = "";
    private string _searchTerm = "";

    private IEnumerable<string> FilteredCommands =>
        string.IsNullOrWhiteSpace(_searchTerm)
            ? WoWMacroConstants.ValidSlashCommands
            : WoWMacroConstants.ValidSlashCommands.Where(cmd =>
                cmd.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("command", out var cmd))
        {
            _selectedCommand = cmd;
        }
    }

    private void HandleSearchInput(ChangeEventArgs e)
    {
        _searchTerm = e.Value?.ToString() ?? "";
        StateHasChanged();
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(_selectedCommand))
        {
            Block.Configuration["command"] = _selectedCommand;
            Block.DisplayText = _selectedCommand;

            await OnSave.InvokeAsync(Block);
        }
    }
}
