using EasyWoWMacro.Business.Models;
using EasyWoWMacro.Business.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.App.Client.Pages;

public partial class MacroSyntaxCheck : ComponentBase
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] private ILogger<MacroSyntaxCheck> Logger { get; set; } = default!;
    [Inject] private IMacroParser MacroParser { get; set; } = default!;
    [Inject] private IMessageService MessageService { get; set; } = default!;
    private string? MacroText { get; set; } = string.Empty;
    
    private Macro? Macro { get; set; }
    
    private void ValidateMacro()
    {
        MessageService.Clear();
        
        try
        {
            var macro = MacroParser.ParseMacro(MacroText);
            Macro = macro;
            Logger.LogInformation("Macro parsed successfully: {@Macro}", macro);
            MessageService.ShowMessageBar("Valid macro", MessageIntent.Success, "macro-syntax-check");
        }
        catch (Exception ex)
        {
            Macro = null;
            Logger.LogError(ex, "Failed to parse macro: {MacroText}", MacroText);
            MessageService.ShowMessageBar("Invalid macro", MessageIntent.Error, "macro-syntax-check");
        }
    }

    private void OnMacroTextChanged(string? obj)
    {
        MessageService.Clear();
        MacroText = obj;
        ValidateMacro();
    }

    private void CopyToClipboard()
    {
        JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", MacroText);
    }
    
    private void TrimMacro()
    {
        MessageService.Clear();
        if (string.IsNullOrWhiteSpace(MacroText))
            return;
        
        ValidateMacro();

        MacroText = Macro!.ToString(true);
    }
}