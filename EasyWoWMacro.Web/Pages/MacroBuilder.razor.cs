using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Validation;

namespace EasyWoWMacro.Web.Pages;

public partial class MacroBuilder
{
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private readonly List<List<MacroBlock>> _macroLines = [];
    private string _macroName = "";
    private List<string> _validationErrors = [];
    private bool _isValid;

    // Modal states
    private bool _showDirectiveModal;
    private bool _showCommandModal;
    private bool _showConditionalModal;
    private bool _showArgumentModal;
    private List<string> _currentConditionalGroup = new();
    private int _editingLineIndex = -1;
    private int _editingBlockIndex = -1;

    protected override void OnInitialized()
    {
        AddNewLine();
    }

    private void AddNewLine()
    {
        _macroLines.Add(new List<MacroBlock>());
        StateHasChanged();
    }

    private void RemoveLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < _macroLines.Count)
        {
            _macroLines.RemoveAt(lineIndex);
            StateHasChanged();
        }
    }

    private void AddBlock(MacroBlock block)
    {
        // Add to the last line, or create a new line if none exist
        if (_macroLines.Count == 0)
        {
            AddNewLine();
        }

        _macroLines[_macroLines.Count - 1].Add(block);
        StateHasChanged();
    }

    // Configuration methods
    private void ConfigureBlock(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Directive:
                _showDirectiveModal = true;
                break;
            case BlockType.Command:
                _showCommandModal = true;
                break;
            case BlockType.ConditionalGroup:
                _showConditionalModal = true;
                _currentConditionalGroup = new List<string>();
                break;
            case BlockType.Argument:
                _showArgumentModal = true;
                break;
        }

        StateHasChanged();
    }

    // Modal save methods
    private void SaveDirective(string directive)
    {
        AddBlock(new MacroBlock(directive, BlockType.Directive));
        HideDirectiveModal();
    }

    private void SaveCommand(string command)
    {
        AddBlock(new MacroBlock(command, BlockType.Command));
        HideCommandModal();
    }

    private void SaveArgument(string argument)
    {
        AddBlock(new MacroBlock(argument, BlockType.Argument));
        HideArgumentModal();
    }

    private void SaveConditionalGroup(List<string> conditionals)
    {
        if (_editingLineIndex >= 0 && _editingBlockIndex >= 0 &&
            _editingLineIndex < _macroLines.Count &&
            _editingBlockIndex < _macroLines[_editingLineIndex].Count)
        {
            // Update existing conditional group
            var groupContent = $"[{string.Join(",", conditionals)}]";
            _macroLines[_editingLineIndex][_editingBlockIndex] = new MacroBlock(groupContent, BlockType.ConditionalGroup);
        }
        else
        {
            // Add new conditional group
            var groupContent = $"[{string.Join(",", conditionals)}]";
            AddBlock(new MacroBlock(groupContent, BlockType.ConditionalGroup));
        }

        HideConditionalModal();
    }

    // Modal hide methods
    private void HideDirectiveModal()
    {
        _showDirectiveModal = false;
        StateHasChanged();
    }

    private void HideCommandModal()
    {
        _showCommandModal = false;
        StateHasChanged();
    }

    private void HideConditionalModal()
    {
        _showConditionalModal = false;
        _currentConditionalGroup = new List<string>();
        _editingLineIndex = -1;
        _editingBlockIndex = -1;
        StateHasChanged();
    }

    private void HideArgumentModal()
    {
        _showArgumentModal = false;
        StateHasChanged();
    }

    private string GetBlockClass(BlockType type)
    {
        return type switch
        {
            BlockType.Directive => "bg-info",
            BlockType.Command => "bg-primary",
            BlockType.Conditional => "bg-warning",
            BlockType.ConditionalGroup => "bg-warning",
            BlockType.Argument => "bg-secondary",
            _ => "bg-secondary"
        };
    }

    private string GetGeneratedMacro()
    {
        var lines = new List<string>();

        foreach (var line in _macroLines)
        {
            if (line.Count > 0)
            {
                var lineContent = string.Join(" ", line.Select(block => block.Content));
                lines.Add(lineContent);
            }
        }

        return string.Join("\n", lines);
    }

    private int GetCharacterCount()
    {
        return GetGeneratedMacro().Length;
    }

    private string GetCharacterCounterClass()
    {
        var count = GetCharacterCount();
        if (count > 255) return "text-danger";
        if (count > 230) return "text-warning";
        return "text-muted";
    }

    private async Task CopyToClipboard()
    {
        var macro = GetGeneratedMacro();
        if (!string.IsNullOrEmpty(macro))
        {
            await Js.InvokeVoidAsync("navigator.clipboard.writeText", macro);
        }
    }

    private void ValidateMacro()
    {
        var macro = GetGeneratedMacro();
        if (string.IsNullOrWhiteSpace(macro))
        {
            _validationErrors = new List<string> { "Macro is empty" };
            _isValid = false;
            return;
        }

        try
        {
            _validationErrors = SyntaxValidator.ValidateMacroText(macro);
            _isValid = _validationErrors.Count == 0;
        }
        catch (Exception ex)
        {
            _validationErrors = new List<string> { $"Parsing error: {ex.Message}" };
            _isValid = false;
        }

        StateHasChanged();
    }

    private void ClearMacro()
    {
        _macroLines.Clear();
        _macroName = "";
        _validationErrors.Clear();
        _isValid = false;
        AddNewLine();
        StateHasChanged();
    }

    private void LoadTemplate(string templateName)
    {
        ClearMacro();

        switch (templateName.ToLower())
        {
            case "mouseover-heal":
                _macroLines[0].Add(new MacroBlock("/cast", BlockType.Command));
                _macroLines[0].Add(new MacroBlock("@mouseover", BlockType.Argument));
                _macroLines[0].Add(new MacroBlock("Heal", BlockType.Argument));
                break;

            case "combat-item":
                _macroLines[0].Add(new MacroBlock("[combat]", BlockType.ConditionalGroup));
                _macroLines[0].Add(new MacroBlock("/use", BlockType.Command));
                _macroLines[0].Add(new MacroBlock("Healthstone", BlockType.Argument));
                break;

            case "modifier-spell":
                _macroLines[0].Add(new MacroBlock("[mod:alt]", BlockType.ConditionalGroup));
                _macroLines[0].Add(new MacroBlock("/cast", BlockType.Command));
                _macroLines[0].Add(new MacroBlock("Alt Spell", BlockType.Argument));
                _macroLines.Add(new List<MacroBlock>());
                _macroLines[1].Add(new MacroBlock("/cast", BlockType.Command));
                _macroLines[1].Add(new MacroBlock("Normal Spell", BlockType.Argument));
                break;

            case "target-switch":
                _macroLines[0].Add(new MacroBlock("/target", BlockType.Command));
                _macroLines[0].Add(new MacroBlock("target", BlockType.Argument));
                _macroLines.Add(new List<MacroBlock>());
                _macroLines[1].Add(new MacroBlock("/target", BlockType.Command));
                _macroLines[1].Add(new MacroBlock("focus", BlockType.Argument));
                break;
        }

        StateHasChanged();
    }

    private void EditBlock(int lineIndex, int blockIndex, BlockType type)
    {
        Console.WriteLine($"EditBlock called with lineIndex: {lineIndex}, blockIndex: {blockIndex}, type: {type}");
        
        if (lineIndex >= 0 && lineIndex < _macroLines.Count &&
            blockIndex >= 0 && blockIndex < _macroLines[lineIndex].Count)
        {
            Console.WriteLine($"EditBlock: Valid indices, setting up modal for type: {type}");
            _editingLineIndex = lineIndex;
            _editingBlockIndex = blockIndex;

            switch (type)
            {
                case BlockType.Directive:
                    Console.WriteLine("EditBlock: Showing directive modal");
                    _showDirectiveModal = true;
                    break;
                case BlockType.Command:
                    Console.WriteLine("EditBlock: Showing command modal");
                    _showCommandModal = true;
                    break;
                case BlockType.ConditionalGroup:
                    Console.WriteLine("EditBlock: Showing conditional modal");
                    _showConditionalModal = true;
                    // Parse the conditional group content to extract individual conditionals
                    var block = _macroLines[lineIndex][blockIndex];
                    var content = block.Content.Trim('[', ']');
                    _currentConditionalGroup = content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToList();
                    break;
                case BlockType.Argument:
                    Console.WriteLine("EditBlock: Showing argument modal");
                    _showArgumentModal = true;
                    break;
            }

            StateHasChanged();
            Console.WriteLine("EditBlock: StateHasChanged called");
        }
        else
        {
            Console.WriteLine($"EditBlock: Invalid indices - lineIndex: {lineIndex}, blockIndex: {blockIndex}, macroLines.Count: {_macroLines.Count}");
        }
    }

    private void SaveEditDirective(string directive)
    {
        if (_editingLineIndex >= 0 && _editingBlockIndex >= 0)
        {
            _macroLines[_editingLineIndex][_editingBlockIndex] = new MacroBlock(directive, BlockType.Directive);
        }
        HideDirectiveModal();
    }
    private void SaveEditCommand(string command)
    {
        if (_editingLineIndex >= 0 && _editingBlockIndex >= 0)
        {
            _macroLines[_editingLineIndex][_editingBlockIndex] = new MacroBlock(command, BlockType.Command);
        }
        HideCommandModal();
    }
    private void SaveEditArgument(string argument)
    {
        if (_editingLineIndex >= 0 && _editingBlockIndex >= 0)
        {
            _macroLines[_editingLineIndex][_editingBlockIndex] = new MacroBlock(argument, BlockType.Argument);
        }
        HideArgumentModal();
    }
    private void SaveEditConditionalGroup(List<string> conditionals)
    {
        if (_editingLineIndex >= 0 && _editingBlockIndex >= 0)
        {
            var groupContent = $"[{string.Join(",", conditionals)}]";
            _macroLines[_editingLineIndex][_editingBlockIndex] = new MacroBlock(groupContent, BlockType.ConditionalGroup);
        }
        HideConditionalModal();
    }

    private void OnDropOnLineWithBlockType(int lineIndex, BlockType blockType)
    {
        Console.WriteLine($"OnDropOnLineWithBlockType called for line {lineIndex} with block type: {blockType}");
        Console.WriteLine($"Current macro lines count: {_macroLines.Count}");
        Console.WriteLine($"Line index valid: {lineIndex >= 0 && lineIndex < _macroLines.Count}");
        
        if (lineIndex >= 0 && lineIndex < _macroLines.Count)
        {
            var placeholder = blockType switch
            {
                BlockType.Directive => "#showtooltip",
                BlockType.Command => "/cast",
                BlockType.ConditionalGroup => "[combat]",
                BlockType.Argument => "Spell Name",
                _ => "Text"
            };

            Console.WriteLine($"Adding block with placeholder: {placeholder}");
            _macroLines[lineIndex].Add(new MacroBlock(placeholder, blockType));
            Console.WriteLine($"Line {lineIndex} now has {_macroLines[lineIndex].Count} blocks");
            
            // Force state change
            StateHasChanged();
            Console.WriteLine("StateHasChanged called");
        }
        else
        {
            Console.WriteLine($"Invalid line index: {lineIndex}");
        }
    }

    private string GetMacroKey()
    {
        // Create a unique key based on the macro lines content
        var totalBlocks = _macroLines.Sum(line => line.Count);
        var contentHash = string.Join("|", _macroLines.Select(line => 
            string.Join(",", line.Select(block => $"{block.Type}:{block.Content}"))));
        
        return $"{_macroLines.Count}-{totalBlocks}-{contentHash.GetHashCode()}";
    }

    private void RemoveBlock(int lineIndex, MacroBlock block)
    {
        if (lineIndex >= 0 && lineIndex < _macroLines.Count)
        {
            _macroLines[lineIndex].Remove(block);
            StateHasChanged();
        }
    }
}
