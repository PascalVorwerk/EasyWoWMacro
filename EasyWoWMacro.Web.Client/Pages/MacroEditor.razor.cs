using System.Globalization;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing;
using EasyWoWMacro.Core.Services;
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
    private List<ValidationError> _enhancedValidationErrors = [];
    private readonly MacroParser _parser = new();
    private readonly ValidationEnhancementService _validationEnhancer = new();
    private bool _showCopyToast;
    private string _toastMessage = "";
    private bool _isCopying;
    private bool _showStructureView;

    // Import functionality properties
    private bool _showImportModal;
    private string _importText = "";
    private List<string> _importErrors = [];

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

    #region Import Functionality

    private void ShowImportModal()
    {
        _showImportModal = true;
        _importText = "";
        _importErrors.Clear();
        StateHasChanged();
    }

    private void HideImportModal()
    {
        _showImportModal = false;
        _importText = "";
        _importErrors.Clear();
        StateHasChanged();
    }

    private async Task ImportMacro()
    {
        try
        {
            _importErrors.Clear();

            if (string.IsNullOrWhiteSpace(_importText))
            {
                _importErrors.Add("Please enter macro text to import.");
                StateHasChanged();
                return;
            }

            // Parse the macro text
            var parsedMacro = _parser.Parse(_importText);
            var validationErrors = _parser.ValidateMacroText(_importText);

            if (validationErrors.Count > 0)
            {
                _importErrors.AddRange(validationErrors);
                StateHasChanged();
                return;
            }

            // Convert parsed macro to building blocks
            _macroLines.Clear();
            
            foreach (var macroLine in parsedMacro.Lines)
            {
                var blocks = ConvertMacroLineToBlocks(macroLine);
                if (blocks.Count > 0)
                {
                    _macroLines.Add(blocks);
                }
            }

            // Ensure we have at least one empty line if no content
            if (_macroLines.Count == 0)
            {
                _macroLines.Add([]);
            }

            // Close the modal and refresh the UI
            HideImportModal();
            ParseMacro(); // Immediately parse and validate the imported macro
            
            await ShowSuccessToast("✓ Macro imported successfully!");
        }
        catch (Exception ex)
        {
            _importErrors.Add($"Import failed: {ex.Message}");
            StateHasChanged();
        }
    }

    private List<MacroBlockModel> ConvertMacroLineToBlocks(MacroLine macroLine)
    {
        var blocks = new List<MacroBlockModel>();

        switch (macroLine)
        {
            case DirectiveLine directive:
                blocks.Add(new MacroBlockModel
                {
                    Type = "Directive",
                    DisplayText = directive.Directive + (string.IsNullOrEmpty(directive.Argument) ? "" : $" {directive.Argument}"),
                    Configuration = new Dictionary<string, string>
                    {
                        { "directive", directive.Directive },
                        { "value", directive.Argument ?? "" }
                    }
                });
                break;

            case CommandLine command:
                // Add command block
                blocks.Add(new MacroBlockModel
                {
                    Type = "Command",
                    DisplayText = command.Command,
                    Configuration = new Dictionary<string, string>
                    {
                        { "command", command.Command }
                    }
                });

                // Add clauses as conditional and argument blocks
                foreach (var clause in command.Clauses)
                {
                    // Add conditionals if present
                    if (clause.Conditions != null && clause.Conditions.ConditionSets.Any())
                    {
                        var conditionalsText = ConvertConditionsToText(clause.Conditions);
                        blocks.Add(new MacroBlockModel
                        {
                            Type = "Conditional",
                            DisplayText = conditionalsText,
                            Configuration = new Dictionary<string, string>
                            {
                                { "conditionals", conditionalsText }
                            }
                        });
                    }

                    // Add argument if present
                    if (!string.IsNullOrEmpty(clause.Argument))
                    {
                        blocks.Add(new MacroBlockModel
                        {
                            Type = "Argument",
                            DisplayText = clause.Argument,
                            Configuration = new Dictionary<string, string>
                            {
                                { "value", clause.Argument }
                            }
                        });
                    }
                }
                break;

            case CommentLine comment:
                // Comments aren't typically part of the visual editor, but we could add them as a special block type
                // For now, we'll skip them since the UI doesn't have comment blocks
                break;
        }

        return blocks;
    }

    private string ConvertConditionsToText(Conditional conditional)
    {
        var conditionSets = new List<string>();

        foreach (var conditionSet in conditional.ConditionSets)
        {
            var conditions = new List<string>();
            foreach (var condition in conditionSet.Conditions)
            {
                if (string.IsNullOrEmpty(condition.Value))
                {
                    conditions.Add(condition.Key);
                }
                else
                {
                    conditions.Add($"{condition.Key}:{condition.Value}");
                }
            }
            conditionSets.Add($"[{string.Join(",", conditions)}]");
        }

        return string.Join("", conditionSets);
    }

    #endregion

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

            // Enhance validation errors with contextual help
            _enhancedValidationErrors = _validationEnhancer.EnhanceValidationErrors(_validationErrors, macroText);
        }
        catch (Exception ex)
        {
            _validationErrors.Clear();
            _validationErrors.Add($"Parsing error: {ex.Message}");
            _enhancedValidationErrors = _validationEnhancer.EnhanceValidationErrors(_validationErrors, "");
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


    private string ConvertBlocksToMacroText()
    {
        var lines = new List<string>();

        foreach (var line in _macroLines)
        {
            if (line.Count == 0) continue;

            var lineText = ConvertLineBlocksToText(line);
            if (!string.IsNullOrWhiteSpace(lineText))
            {
                lines.Add(lineText.Trim());
            }
        }

        return string.Join("\n", lines);
    }

    private string ConvertLineBlocksToText(List<MacroBlockModel> line)
    {
        if (line.Count == 0) return "";

        // Handle directive lines
        var firstBlock = line[0];
        if (firstBlock.Type == "Directive")
        {
            if (firstBlock.Configuration.TryGetValue("directive", out var dir))
            {
                if (firstBlock.Configuration.TryGetValue("value", out var value) && !string.IsNullOrEmpty(value))
                {
                    return $"{dir} {value}";
                }
                return dir;
            }
            return "";
        }

        // Handle command lines
        string command = "";
        var blocks = new List<MacroBlockModel>();

        // Extract command and remaining blocks
        for (int i = 0; i < line.Count; i++)
        {
            var block = line[i];
            if (block.Type == "Command" && string.IsNullOrEmpty(command))
            {
                if (block.Configuration.TryGetValue("command", out var cmd))
                {
                    command = cmd;
                }
            }
            else
            {
                blocks.Add(block);
            }
        }

        if (string.IsNullOrEmpty(command)) return "";

        // Convert blocks to clauses
        var clauses = ConvertBlocksToClauses(blocks);
        
        if (clauses.Count == 0)
        {
            return command;
        }

        return $"{command} {string.Join("; ", clauses)}";
    }

    private List<string> ConvertBlocksToClauses(List<MacroBlockModel> blocks)
    {
        var clauses = new List<string>();
        var currentConditionals = new List<string>();
        
        for (int i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];

            if (block.Type == "Conditional")
            {
                if (block.Configuration.TryGetValue("conditionals", out var conditions))
                {
                    currentConditionals.Add(conditions);
                }
            }
            else if (block.Type == "Argument")
            {
                string argument = "";
                if (block.Configuration.TryGetValue("value", out var argValue))
                {
                    argument = argValue;
                }
                else
                {
                    argument = block.DisplayText;
                }

                // Create clause with accumulated conditionals and this argument
                if (currentConditionals.Count > 0)
                {
                    var conditionalsText = string.Join("", currentConditionals);
                    clauses.Add($"{conditionalsText} {argument}");
                    currentConditionals.Clear();
                }
                else
                {
                    // Fallback clause without conditionals
                    clauses.Add(argument);
                }
            }
        }

        // Handle trailing conditionals without arguments (rare case)
        if (currentConditionals.Count > 0)
        {
            var conditionalsText = string.Join("", currentConditionals);
            clauses.Add(conditionalsText);
        }

        return clauses;
    }

    private void ToggleView(bool showStructure)
    {
        _showStructureView = showStructure;
        StateHasChanged();
    }
}
