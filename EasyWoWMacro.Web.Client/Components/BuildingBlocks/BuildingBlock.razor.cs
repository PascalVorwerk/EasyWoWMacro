using EasyWoWMacro.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace EasyWoWMacro.Web.Client.Components.BuildingBlocks;

public partial class BuildingBlock : ComponentBase
{
    [Parameter]
    public string BlockType { get; set; } = "";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnConfigureClick { get; set; }

    private void HandleDragStart(DragEventArgs args)
    {
        DragDropService.DraggedBlockType = BlockType;
        Console.WriteLine($"Drag started for block type: {BlockType}");
    }

    private async Task HandleClick()
    {
        if (OnConfigureClick.HasDelegate)
        {
            await OnConfigureClick.InvokeAsync();
        }
    }
}
