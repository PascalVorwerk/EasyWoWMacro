using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class PreviewExport
{
    [Parameter] public string MacroName { get; set; } = "";
    [Parameter] public string GeneratedMacro { get; set; } = "";
    [Parameter] public List<string> ValidationErrors { get; set; } = new();
    [Parameter] public bool IsValid { get; set; } = false;
    
    [Parameter] public EventCallback OnCopyToClipboard { get; set; }
    [Parameter] public EventCallback OnValidateMacro { get; set; }
    
    private int GetCharacterCount()
    {
        return GeneratedMacro?.Length ?? 0;
    }
    
    private string GetCharacterCounterClass()
    {
        var count = GetCharacterCount();
        if (count > 255) return "danger";
        if (count > 230) return "warning";
        return "";
    }
} 