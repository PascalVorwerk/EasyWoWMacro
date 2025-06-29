using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class DirectiveModal
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<string> OnSelectCallback { get; set; }
    [Parameter] public EventCallback OnCancelCallback { get; set; }
    
    private string SearchTerm { get; set; } = "";
    
    private IEnumerable<string> GetFilteredDirectives()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return WoWMacroConstants.ValidDirectives;
            
        return WoWMacroConstants.ValidDirectives.Where(d => 
            d.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
    }
    
    private async Task OnSelectDirective(string directive)
    {
        await OnSelectCallback.InvokeAsync(directive);
    }
    
    private async Task OnCancel()
    {
        await OnCancelCallback.InvokeAsync();
    }
} 