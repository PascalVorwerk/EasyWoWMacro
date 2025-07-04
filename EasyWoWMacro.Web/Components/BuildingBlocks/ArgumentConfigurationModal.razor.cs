using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Web.Models;

namespace EasyWoWMacro.Web.Components.BuildingBlocks;

public partial class ArgumentConfigurationModal : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public MacroBlockModel? Block { get; set; }

    [Parameter]
    public EventCallback<MacroBlockModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    private string argumentValue = "";

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("value", out var value))
        {
            argumentValue = value;
        }
        else if (Block != null && !string.IsNullOrEmpty(Block.DisplayText) && Block.DisplayText != "Argument")
        {
            argumentValue = Block.DisplayText;
        }
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(argumentValue))
        {
            Block.Configuration["value"] = argumentValue;
            Block.DisplayText = argumentValue;
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 