using System.Globalization;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.Web.Client.Models;
using EasyWoWMacro.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.Web.Client.Pages;

public partial class MacroEditor : ComponentBase
{
    [Inject]
    private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private IConditionalService? ConditionalService { get; set; }

    private readonly List<List<MacroBlockModel>> _macroLines = [new()];
    private Macro? _parsedMacro;
    private List<string> _validationErrors = [];
    private readonly MacroParser _parser = new();
    private bool _showCopyToast;
    private string _toastMessage = "";
    private bool _isCopying;

    private void AddNewLine()
    {
        _macroLines.Add([]);
        StateHasChanged();
    }

    private void DeleteLine(List<MacroBlockModel> line)
    {
        _macroLines.Remove(line);
        if (_macroLines.Count == 0)
        {
            _macroLines.Add([]);
        }
        StateHasChanged();
    }

    private void HandleConfigureBlock(MacroBlockModel block)
    {
        // The configuration is now handled by the specific modals in MacroLine
        StateHasChanged();
    }

    private void ParseMacro()
    {
        try
        {
            var macroText = ConvertBlocksToMacroText();
            _validationErrors = _parser.ValidateMacroText(macroText);

            if (_validationErrors.Count == 0)
            {
                _parsedMacro = _parser.Parse(macroText);
                var macroErrors = _parsedMacro.Validate();
                _validationErrors.AddRange(macroErrors);

                var formattedLength = _parsedMacro.GetFormattedMacro().Length;
                if (formattedLength > 255)
                {
                    _validationErrors.Add($"Macro exceeds WoW's 255 character limit ({formattedLength} characters).");
                }
            }
            else
            {
                _parsedMacro = null;
            }
        }
        catch (Exception ex)
        {
            _validationErrors.Clear();
            _validationErrors.Add($"Parsing error: {ex.Message}");
            _parsedMacro = null;
        }

        StateHasChanged();
    }

    private async Task CopyToClipboard()
    {
        _isCopying = true;
        StateHasChanged();

        try
        {
            string textToCopy;

            if (_parsedMacro != null)
            {
                textToCopy = _parsedMacro.GetFormattedMacro();
            }
            else
            {
                // If no parsed macro, try to generate from current blocks
                textToCopy = ConvertBlocksToMacroText();
                if (string.IsNullOrWhiteSpace(textToCopy))
                {
                    // Show error toast
                    await ShowErrorToast("No macro content to copy");
                    return;
                }
            }

            // Log what we're trying to copy
            await Js.InvokeVoidAsync("console.log", $"Attempting to copy: {textToCopy}");

            // Try modern clipboard API first
            try
            {
                await Js.InvokeVoidAsync("navigator.clipboard.writeText", textToCopy);
                await ShowSuccessToast("✓ Copied to clipboard!");
            }
            catch (Exception ex)
            {
                // Log the error
                await Js.InvokeVoidAsync("console.log", $"Modern clipboard API failed: {ex.Message}");

                // Fallback to older method
                await Js.InvokeVoidAsync("copyToClipboard", textToCopy);
                await ShowSuccessToast("✓ Copied to clipboard!");
            }
        }
        catch (Exception ex)
        {
            await Js.InvokeVoidAsync("console.log", $"Copy failed: {ex.Message}");
            await ShowErrorToast($"Failed to copy: {ex.Message}");
        }
        finally
        {
            _isCopying = false;
            StateHasChanged();
        }
    }

    private async Task ShowSuccessToast(string message)
    {
        _showCopyToast = true;
        _toastMessage = message;
        StateHasChanged();

        // Hide toast after 3 seconds
        await Task.Delay(3000);
        _showCopyToast = false;
        StateHasChanged();
    }

    private async Task ShowErrorToast(string message)
    {
        // For now, just use the same toast but we could add error styling
        _showCopyToast = true;
        _toastMessage = message;
        StateHasChanged();

        await Task.Delay(3000);
        _showCopyToast = false;
        StateHasChanged();
    }

    private string GetFormattedMacroLength()
    {
        if (_parsedMacro == null) return "0";
        var formatted = _parsedMacro.GetFormattedMacro().Length;
        return formatted.ToString(CultureInfo.InvariantCulture);
    }

    private string ConvertBlocksToMacroText()
    {
        var lines = new List<string>();

        foreach (var line in _macroLines)
        {
            if (line.Count == 0) continue;

            var lineText = "";
            foreach (var block in line)
            {
                switch (block.Type)
                {
                    case "Command":
                        if (block.Configuration.TryGetValue("command", out var cmd))
                        {
                            lineText += $"{cmd} ";
                        }
                        break;
                    case "Conditional":
                        if (block.Configuration.TryGetValue("conditionals", out var conditions))
                        {
                            lineText += $"{conditions} ";
                        }
                        break;
                    case "Directive":
                        if (block.Configuration.TryGetValue("directive", out var dir))
                        {
                            if (block.Configuration.TryGetValue("value", out var value) && !string.IsNullOrEmpty(value))
                            {
                                lineText += $"{dir} {value} ";
                            }
                            else
                            {
                                lineText += $"{dir} ";
                            }
                        }
                        break;
                    case "Argument":
                        if (block.Configuration.TryGetValue("value", out var argValue))
                        {
                            lineText += $"{argValue} ";
                        }
                        else
                        {
                            lineText += $"{block.DisplayText} ";
                        }
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(lineText))
            {
                lines.Add(lineText.Trim());
            }
        }

        return string.Join("\n", lines);
    }
}
