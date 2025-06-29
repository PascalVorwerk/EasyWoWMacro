using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class BuildingBlocks : IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    
    private IJSObjectReference? module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Load the collocated JavaScript module
                module = await JS.InvokeAsync<IJSObjectReference>("import", 
                    "./Pages/Components/BuildingBlocks.razor.js");
                
                // Initialize drag handlers
                await module.InvokeVoidAsync("initializeDragHandlers");
                
                Console.WriteLine("BuildingBlocks: JavaScript module loaded and drag handlers initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BuildingBlocks: Error loading JavaScript module: {ex.Message}");
            }
        }
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
    }
}
