using EasyWoWMacro.Web.Client.Models;
using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Client.Components.Modals;

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

    private string _selectedDirective = "";
    private string _directiveValue = "";

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("directive", out var directive))
        {
            _selectedDirective = directive;
        }
        if (Block != null && Block.Configuration.TryGetValue("value", out var value))
        {
            _directiveValue = value;
        }
    }

    private async Task HandleSave()
    {
        if (Block != null && !string.IsNullOrEmpty(_selectedDirective))
        {
            Block.Configuration["directive"] = _selectedDirective;
            Block.Configuration["value"] = _directiveValue;

            if (!string.IsNullOrEmpty(_directiveValue))
            {
                Block.DisplayText = $"{_selectedDirective} {_directiveValue}";
            }
            else
            {
                Block.DisplayText = _selectedDirective;
            }

            await OnSave.InvokeAsync(Block);
        }
    }
}
