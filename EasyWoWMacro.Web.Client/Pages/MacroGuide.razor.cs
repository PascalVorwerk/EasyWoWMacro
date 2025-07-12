using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.Web.Client.Pages;

public partial class MacroGuide : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    private IJSObjectReference _module = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/MacroGuide.razor.js");
        }
    }

    private async Task ScrollToSectionAsync(string sectionId)
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("scrollToSection", sectionId);
        }
    }
}