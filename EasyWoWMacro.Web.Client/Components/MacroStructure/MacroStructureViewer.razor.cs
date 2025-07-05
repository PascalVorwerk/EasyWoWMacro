using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Client.Components.MacroStructure;

public partial class MacroStructureViewer : ComponentBase
{
    [Parameter] public List<MacroLine>? MacroLines { get; set; }
    
    private bool IsExpanded { get; set; } = true;
    
    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }
} 