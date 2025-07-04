using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EasyWoWMacro.Web.Models;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace EasyWoWMacro.Web.Pages;

public partial class MacroEditor : ComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private List<List<MacroBlockModel>> macroLines = new() { new() };
    private Macro? parsedMacro;
    private List<string> validationErrors = new();
    private MacroParser parser = new();
    private bool showCopyToast = false;
    private string toastMessage = "";
    private bool isCopying = false;

    private void AddNewLine()
    {
        macroLines.Add(new());
        StateHasChanged();
    }

    private void DeleteLine(List<MacroBlockModel> line)
    {
        macroLines.Remove(line);
        if (macroLines.Count == 0)
        {
            macroLines.Add(new());
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
            validationErrors = parser.ValidateMacroText(macroText);

            if (validationErrors.Count == 0)
            {
                parsedMacro = parser.Parse(macroText);
                var macroErrors = parsedMacro.Validate();
                validationErrors.AddRange(macroErrors);

                var formattedLength = parsedMacro.GetFormattedMacro().Length;
                if (formattedLength > 255)
                {
                    validationErrors.Add($"Macro exceeds WoW's 255 character limit ({formattedLength} characters).");
                }
            }
            else
            {
                parsedMacro = null;
            }
        }
        catch (Exception ex)
        {
            validationErrors.Clear();
            validationErrors.Add($"Parsing error: {ex.Message}");
            parsedMacro = null;
        }
        
        StateHasChanged();
    }

    private async Task CopyToClipboard()
    {
        isCopying = true;
        StateHasChanged();
        
        try
        {
            string textToCopy;
            
            if (parsedMacro != null)
            {
                textToCopy = parsedMacro.GetFormattedMacro();
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
            await JS.InvokeVoidAsync("console.log", $"Attempting to copy: {textToCopy}");

            // Try modern clipboard API first
            try
            {
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", textToCopy);
                await ShowSuccessToast("✓ Copied to clipboard!");
            }
            catch (Exception ex)
            {
                // Log the error
                await JS.InvokeVoidAsync("console.log", $"Modern clipboard API failed: {ex.Message}");
                
                // Fallback to older method
                await JS.InvokeVoidAsync("copyToClipboard", textToCopy);
                await ShowSuccessToast("✓ Copied to clipboard!");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("console.log", $"Copy failed: {ex.Message}");
            await ShowErrorToast($"Failed to copy: {ex.Message}");
        }
        finally
        {
            isCopying = false;
            StateHasChanged();
        }
    }

    private async Task ShowSuccessToast(string message)
    {
        showCopyToast = true;
        toastMessage = message;
        StateHasChanged();
        
        // Hide toast after 3 seconds
        await Task.Delay(3000);
        showCopyToast = false;
        StateHasChanged();
    }

    private async Task ShowErrorToast(string message)
    {
        // For now, just use the same toast but we could add error styling
        showCopyToast = true;
        toastMessage = message;
        StateHasChanged();
        
        await Task.Delay(3000);
        showCopyToast = false;
        StateHasChanged();
    }

    private string GetFormattedMacroLength()
    {
        if (parsedMacro == null) return "0";
        var formatted = parsedMacro.GetFormattedMacro().Length;
        return formatted.ToString(CultureInfo.InvariantCulture);
    }

    private string ConvertBlocksToMacroText()
    {
        var lines = new List<string>();
        
        foreach (var line in macroLines)
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
                        if (block.Configuration.TryGetValue("conditions", out var conditions))
                        {
                            var conditionList = conditions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                            var formattedConditions = new List<string>();
                            foreach (var condition in conditionList)
                            {
                                if (condition == "mod:" && block.Configuration.TryGetValue("modifier", out var modifier))
                                {
                                    formattedConditions.Add($"mod:{modifier}");
                                }
                                else if (condition == "nomod:" && block.Configuration.TryGetValue("modifier", out var nomodifier))
                                {
                                    formattedConditions.Add($"nomod:{nomodifier}");
                                }
                                else
                                {
                                    formattedConditions.Add(condition);
                                }
                            }
                            // Output as a single group: [cond1, cond2, cond3]
                            lineText += $"[{string.Join(", ", formattedConditions)}] ";
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