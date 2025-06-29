using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class ArgumentModal
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<string> OnSaveCallback { get; set; }
    [Parameter] public EventCallback OnCancelCallback { get; set; }
    
    private string ArgumentValue { get; set; } = "";
    
    private static readonly string[] CommonArguments = 
    {
        "target", "mouseover", "focus", "player", "cursor",
        "alt", "ctrl", "shift",
        "1", "2", "3", "4", "5",
        "weapon", "offhand", "shield", "2h", "1h", "ranged", "ammo",
        "bear", "cat", "travel", "aquatic", "flight", "moonkin", "tree", "battle", "defensive", "berserker"
    };
    
    private void SelectCommonArgument(string argument)
    {
        ArgumentValue = argument;
    }
    
    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(ArgumentValue))
        {
            OnSave();
        }
    }
    
    private async Task OnSave()
    {
        if (!string.IsNullOrWhiteSpace(ArgumentValue))
        {
            await OnSaveCallback.InvokeAsync(ArgumentValue);
        }
    }
    
    private async Task OnCancel()
    {
        await OnCancelCallback.InvokeAsync();
    }
} 