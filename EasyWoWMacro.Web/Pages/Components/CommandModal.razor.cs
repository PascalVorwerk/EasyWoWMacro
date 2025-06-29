using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class CommandModal
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<string> OnSelectCallback { get; set; }
    [Parameter] public EventCallback OnCancelCallback { get; set; }
    
    private string SearchTerm { get; set; } = "";
    
    private IEnumerable<string> GetFilteredCommands()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return WoWMacroConstants.ValidSlashCommands;
            
        return WoWMacroConstants.ValidSlashCommands.Where(c => 
            c.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
    }
    
    private async Task OnSelectCommand(string command)
    {
        await OnSelectCallback.InvokeAsync(command);
    }
    
    private async Task OnCancel()
    {
        await OnCancelCallback.InvokeAsync();
    }
} 