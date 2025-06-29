using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class MacroConstruction : IAsyncDisposable
{
    [Parameter] public List<List<MacroBlock>> MacroLines { get; set; } = new();
    [Parameter] public EventCallback OnAddLine { get; set; }
    [Parameter] public EventCallback OnClearMacro { get; set; }
    [Parameter] public EventCallback<(int lineIndex, MacroBlock block)> OnRemoveBlock { get; set; }
    [Parameter] public EventCallback<int> OnRemoveLine { get; set; }
    [Parameter] public EventCallback<(int lineIndex, BlockType blockType)> OnDropOnLineWithBlockType { get; set; }
    [Parameter] public EventCallback<(int lineIndex, int blockIndex, BlockType type)> OnEditBlock { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = null!;
    
    private IJSObjectReference? module;
    private DotNetObjectReference<MacroConstruction>? dotNetHelper;

    private static string GetBlockClass(BlockType type)
    {
        return type switch
        {
            BlockType.Directive => "bg-info",
            BlockType.Command => "bg-primary",
            BlockType.Conditional => "bg-warning",
            BlockType.ConditionalGroup => "bg-warning",
            _ => "bg-secondary"
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Create the dotnet helper
                dotNetHelper = DotNetObjectReference.Create(this);
                
                // Load the collocated JavaScript module
                module = await JS.InvokeAsync<IJSObjectReference>("import", 
                    "./Pages/Components/MacroConstruction.razor.js");
                
                Console.WriteLine("MacroConstruction: JavaScript module loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MacroConstruction: Error loading JavaScript module: {ex.Message}");
            }
        }
        
        // Set up drop handlers after each render
        await SetupDropHandlers();
    }

    private async Task SetupDropHandlers()
    {
        if (module is not null && dotNetHelper is not null)
        {
            await module.InvokeVoidAsync("setupDropHandlersForAllLines", dotNetHelper);
        }
    }

    [JSInvokable]
    public async Task OnDropWithData(int lineIndex, string blockTypeString)
    {
        Console.WriteLine($"OnDropWithData called for line {lineIndex} with block type: {blockTypeString}");
        
        // Parse the block type string to enum
        if (Enum.TryParse<BlockType>(blockTypeString, out var blockType))
        {
            Console.WriteLine($"Successfully parsed block type: {blockType}");
            // Call the parent component's drop handler with both line index and block type
            await OnDropOnLineWithBlockType.InvokeAsync((lineIndex, blockType));
        }
        else
        {
            Console.WriteLine($"Failed to parse block type: {blockTypeString}");
        }
    }

    private void TestClick(int lineIndex, int blockIndex, BlockType type)
    {
        Console.WriteLine($"TestClick called: line {lineIndex}, block {blockIndex}, type {type}");
        OnEditBlock.InvokeAsync((lineIndex, blockIndex, type));
    }

    public async ValueTask DisposeAsync()
    {
        if (module is not null)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Handle disconnection gracefully
            }
        }
        
        dotNetHelper?.Dispose();
    }
}
