using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Web.Models;

namespace EasyWoWMacro.Web.Components.BuildingBlocks;

public partial class DirectiveConfigurationModal : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public MacroBlockModel? Block { get; set; }

    [Parameter]
    public EventCallback<MacroBlockModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    private string selectedDirective = "";
    private string directiveValue = "";

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("directive", out var directive))
        {
            selectedDirective = directive;
        }
        if (Block != null && Block.Configuration.TryGetValue("value", out var value))
        {
            directiveValue = value;
        }
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(selectedDirective))
        {
            Block.Configuration["directive"] = selectedDirective;
            Block.Configuration["value"] = directiveValue;
            
            if (!string.IsNullOrEmpty(directiveValue))
            {
                Block.DisplayText = $"{selectedDirective} {directiveValue}";
            }
            else
            {
                Block.DisplayText = selectedDirective;
            }
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 