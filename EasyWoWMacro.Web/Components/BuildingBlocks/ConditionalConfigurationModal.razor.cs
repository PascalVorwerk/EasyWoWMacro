using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EasyWoWMacro.Web.Models;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Components.BuildingBlocks;

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

    private string selectedConditionsString = "";
    private string selectedModifier = "";

    private readonly string[] basicConditions = 
    {
        "combat", "nocombat", "harm", "help", "dead", "nodead", 
        "stealth", "nostealth", "mounted", "nomounted", "flying", "noflying",
        "swimming", "noswimming", "indoors", "outdoors", "group", "raid", 
        "party", "solo", "pet", "nopet", "exists", "noexists", "player"
    };

    private readonly string[] advancedConditions = 
    {
        "mod:", "nomod:", "button:", "form:", "stance:", "equipped:", 
        "worn:", "threat:", "target", "mouseover", "focus", "cursor"
    };

    private List<string> SelectedConditions => 
        selectedConditionsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("conditions", out var conditions))
        {
            selectedConditionsString = conditions;
        }
        if (Block != null && Block.Configuration.TryGetValue("modifier", out var modifier))
        {
            selectedModifier = modifier;
        }
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
        
        selectedConditionsString = string.Join(",", currentList);
    }

    private string GetConditionalPreview()
    {
        var conditions = SelectedConditions;
        if (conditions.Count == 0) return "[No conditions selected]";
        
        var formattedConditions = new List<string>();
        foreach (var condition in conditions)
        {
            if (condition == "mod:" && !string.IsNullOrEmpty(selectedModifier))
            {
                formattedConditions.Add($"mod:{selectedModifier}");
            }
            else if (condition == "nomod:" && !string.IsNullOrEmpty(selectedModifier))
            {
                formattedConditions.Add($"nomod:{selectedModifier}");
            }
            else
            {
                formattedConditions.Add(condition);
            }
        }
        // Output as a single group: [cond1, cond2, cond3]
        return $"[{string.Join(", ", formattedConditions)}]";
    }

    private async Task HandleSave()
    {
        if (Block != null && SelectedConditions.Count > 0)
        {
            Block.Configuration["conditions"] = selectedConditionsString;
            Block.Configuration["modifier"] = selectedModifier;
            Block.DisplayText = GetConditionalPreview();
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 