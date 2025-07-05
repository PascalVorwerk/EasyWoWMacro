using EasyWoWMacro.Web.Client.Models;
using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Client.Components.Modals;

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

    private string _argumentValue = "";

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("value", out var value))
        {
            _argumentValue = value;
        }
        else if (Block != null && !string.IsNullOrEmpty(Block.DisplayText) && Block.DisplayText != "Argument")
        {
            _argumentValue = Block.DisplayText;
        }
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(_argumentValue))
        {
            Block.Configuration["value"] = _argumentValue;
            Block.DisplayText = _argumentValue;

            await OnSave.InvokeAsync(Block);
        }
    }
}
