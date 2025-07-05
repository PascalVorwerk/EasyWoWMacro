using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Web.Client.Models;
using EasyWoWMacro.Web.Client.Services;
using System.Text.RegularExpressions;

namespace EasyWoWMacro.Web.Client.Components.BuildingBlocks;

public partial class ConditionalConfigurationModal : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public MacroBlockModel? Block { get; set; }

    [Parameter]
    public EventCallback<MacroBlockModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Inject]
    private IConditionalService? ConditionalService { get; set; }

    private string _selectedConditionsString = "";
    private string _equippedType = "slot";
    private string _equippedSlot = "";
    private string _equippedItem = "";
    private readonly List<string> _validationErrors = [];

    // Search functionality
    private string _basicConditionSearch = "";
    private string _advancedConditionSearch = "";

    // Use service methods instead of hardcoded arrays
    private IEnumerable<string> BasicConditions => ConditionalService?.GetAllBasicConditions().Where(c =>
        string.IsNullOrEmpty(_basicConditionSearch) ||
        c.Contains(_basicConditionSearch, StringComparison.OrdinalIgnoreCase)) ?? [];

    private IEnumerable<string> AdvancedConditions =>
        ConditionalService?.GetAdvancedConditions().Where(c =>
            string.IsNullOrEmpty(_advancedConditionSearch) ||
            c.Contains(_advancedConditionSearch, StringComparison.OrdinalIgnoreCase)) ?? [];

    private List<string> SelectedConditions =>
        string.IsNullOrEmpty(_selectedConditionsString)
            ? []
            : _selectedConditionsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();

    private IEnumerable<ConditionalConfigurationInfo> SelectedConditionConfigurations =>
        ConditionalService?.GetConditionalConfigurationInfo()
            .Where(info => SelectedConditions.Contains(info.Conditional))
            .OrderBy(info => info.Label) ?? Enumerable.Empty<ConditionalConfigurationInfo>();

    private Dictionary<string, string> _advancedValues = new();

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("conditionals", out var condStr))
        {
            _selectedConditionsString = condStr;
            ParseConditionalsString(condStr);
        }
        else
        {
            _selectedConditionsString = "";
            _advancedValues.Clear();
        }
    }

    private void ParseConditionalsString(string condStr)
    {
        _selectedConditionsString = "";
        _advancedValues.Clear();
        var match = Regex.Match(condStr, "\\[(.*?)\\]");
        if (!match.Success) return;
        var inner = match.Groups[1].Value;
        var parts = inner.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
        var selected = new List<string>();
        foreach (var part in parts)
        {
            var idx = part.IndexOf(':');
            if (idx > 0)
            {
                var key = part.Substring(0, idx + 1); // include colon
                var value = part.Substring(idx + 1);
                selected.Add(key);
                _advancedValues[key] = value;
            }
            else
            {
                selected.Add(part);
            }
        }
        _selectedConditionsString = string.Join(",", selected);
    }

    private string GetConditionalPreview()
    {
        var conditions = SelectedConditions;
        if (conditions.Count == 0) return "[No conditions selected]";
        var formatted = new List<string>();
        foreach (var cond in conditions)
        {
            if (_advancedValues.TryGetValue(cond, out var val) && !string.IsNullOrEmpty(val))
                formatted.Add($"{cond}{val}");
            else
                formatted.Add(cond);
        }
        return $"[{string.Join(", ", formatted)}]";
    }

    private async Task HandleSave()
    {
        _validationErrors.Clear();
        // Validate: (optional, can add more logic)
        if (_selectedConditionsString.Length == 0)
        {
            _validationErrors.Add("Select at least one condition.");
            return;
        }

        if (Block != null)
        {
            Block.DisplayText = GetConditionalPreview();
            Block.Configuration["conditionals"] = GetConditionalPreview();
        }
        await OnSave.InvokeAsync(Block);
    }

    private void HandleConditionChange(string condition, ChangeEventArgs e)
    {
        var isChecked = e.Value is bool checkedValue && checkedValue;
        UpdateSelectedConditions(condition, isChecked);
    }

    private void UpdateSelectedConditions(string condition, bool isChecked)
    {
        var currentList = SelectedConditions;

        if (isChecked && !currentList.Contains(condition))
        {
            currentList.Add(condition);
        }
        else if (!isChecked && currentList.Contains(condition))
        {
            currentList.Remove(condition);
        }

        _selectedConditionsString = string.Join(",", currentList);
    }

    private async Task HandleClose()
    {
        await OnClose.InvokeAsync();
    }

    private string GetConfigurationValue(string configKey)
    {
        return _advancedValues.TryGetValue(configKey, out var value) ? value : string.Empty;
    }

    private void SetConfigurationValue(string configKey, string value)
    {
        _advancedValues[configKey] = value;
    }
}
