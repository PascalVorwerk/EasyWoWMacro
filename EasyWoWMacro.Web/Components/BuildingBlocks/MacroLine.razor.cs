using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EasyWoWMacro.Web.Services;
using EasyWoWMacro.Web.Models;

namespace EasyWoWMacro.Web.Components.BuildingBlocks;

public partial class MacroLine : ComponentBase
{
    private bool IsOver { get; set; }
    private string? activeModalType;
    private MacroBlockModel? selectedBlock;

    [Parameter]
    public List<MacroBlockModel> Blocks { get; set; } = new();

    [Parameter]
    public EventCallback<MacroBlockModel> OnConfigureBlock { get; set; }

    [Parameter]
    public EventCallback OnDeleteLine { get; set; }

    private void HandleDragOver(DragEventArgs args)
    {
        IsOver = true;
    }

    private void HandleDragLeave()
    {
        IsOver = false;
    }

    private void HandleDrop(DragEventArgs args)
    {
        IsOver = false;
        var blockType = DragDropService.DraggedBlockType;
        Console.WriteLine($"Drop event triggered. Block type: {blockType}");
        
        if (!string.IsNullOrEmpty(blockType))
        {
            var newBlock = new MacroBlockModel
            {
                Type = blockType,
                DisplayText = blockType // This will be updated when configured
            };
            Blocks.Add(newBlock);
            
            // Show the appropriate modal based on block type
            selectedBlock = newBlock;
            activeModalType = blockType;
            
            DragDropService.DraggedBlockType = null;
            Console.WriteLine($"Block added: {blockType}");
        }
        else
        {
            Console.WriteLine("No block type found in DragDropService");
        }
    }

    private void DeleteBlock(MacroBlockModel block)
    {
        Blocks.Remove(block);
        StateHasChanged();
    }

    private void OpenBlockModal(MacroBlockModel block)
    {
        selectedBlock = block;
        activeModalType = block.Type;
        StateHasChanged();
    }

    private async Task HandleBlockConfigSave(MacroBlockModel block)
    {
        activeModalType = null;
        selectedBlock = null;
        
        await OnConfigureBlock.InvokeAsync(block);
        StateHasChanged();
    }

    private void CloseModal()
    {
        activeModalType = null;
        selectedBlock = null;
        StateHasChanged();
    }
} 