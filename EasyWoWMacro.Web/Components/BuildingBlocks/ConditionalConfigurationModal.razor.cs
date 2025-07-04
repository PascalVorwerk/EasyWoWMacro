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
    private string stanceValue = "";
    private string equippedType = "slot";
    private string equippedSlot = "";
    private string equippedItem = "";
    private string formValue = "";
    private string buttonValue = "";
    private string threatValue = "";
    private string specValue = "";
    private string talentValue = "";
    private string glyphValue = "";
    private string spellValue = "";
    private string itemValue = "";
    private string auraValue = "";
    private string buffValue = "";
    private List<string> validationErrors = new();

    private readonly string[] basicConditions = 
    {
        "combat", "nocombat", "harm", "noharm", "help", "nohelp", "dead", "nodead", 
        "stealth", "nostealth", "mounted", "nomounted", "flying", "noflying",
        "swimming", "noswimming", "indoors", "outdoors", "group", "nogroup", "raid", "noraid", 
        "party", "noparty", "solo", "pet", "nopet", "exists", "noexists", "player", "noplayer",
        "worn", "noworn", "threat", "nothreat", "channeling", "nochanneling", "casting", "nocasting",
        "actionbar", "noactionbar", "possessbar", "nopossessbar", "overridebar", "nooverridebar",
        "extrabar", "noextrabar", "vehicleui", "novehicleui", "bonusbar", "nobonusbar",
        "petbattle", "nopetbattle", "modifier", "nomodifier", "spec", "nospec", "talent", "notalent",
        "glyph", "noglyph", "spell", "nospell", "item", "noitem", "equipped", "noequipped",
        "stance", "nostance", "form", "noform", "aura", "noaura", "buff", "nobuff", "debuff", "nodebuff"
    };

    private readonly string[] advancedConditions = 
    {
        "mod:", "nomod:", "button:", "form:", "equipped:", "worn:", "threat:", 
        "@target", "@mouseover", "@focus", "@cursor", "@player", "@npc", "@pet",
        "spec:", "talent:", "glyph:", "spell:", "item:", "aura:", "buff:", "debuff:"
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
        if (Block != null && Block.Configuration.TryGetValue("stance", out var stance))
        {
            stanceValue = stance;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedType", out var eqType))
        {
            equippedType = eqType;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedSlot", out var eqSlot))
        {
            equippedSlot = eqSlot;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedItem", out var eqItem))
        {
            equippedItem = eqItem;
        }
        if (Block != null && Block.Configuration.TryGetValue("form", out var form))
        {
            formValue = form;
        }
        if (Block != null && Block.Configuration.TryGetValue("button", out var button))
        {
            buttonValue = button;
        }
        if (Block != null && Block.Configuration.TryGetValue("threat", out var threat))
        {
            threatValue = threat;
        }
        if (Block != null && Block.Configuration.TryGetValue("spec", out var spec))
        {
            specValue = spec;
        }
        if (Block != null && Block.Configuration.TryGetValue("talent", out var talent))
        {
            talentValue = talent;
        }
        if (Block != null && Block.Configuration.TryGetValue("glyph", out var glyph))
        {
            glyphValue = glyph;
        }
        if (Block != null && Block.Configuration.TryGetValue("spell", out var spell))
        {
            spellValue = spell;
        }
        if (Block != null && Block.Configuration.TryGetValue("item", out var item))
        {
            itemValue = item;
        }
        if (Block != null && Block.Configuration.TryGetValue("aura", out var aura))
        {
            auraValue = aura;
        }
        if (Block != null && Block.Configuration.TryGetValue("buff", out var buff))
        {
            buffValue = buff;
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
            else if (condition == "stance:" && !string.IsNullOrEmpty(stanceValue))
            {
                formattedConditions.Add($"stance:{stanceValue}");
            }
            else if (condition == "equipped:" && !string.IsNullOrEmpty(equippedType))
            {
                if (equippedType == "slot" && !string.IsNullOrEmpty(equippedSlot))
                {
                    formattedConditions.Add($"equipped:{equippedSlot}");
                }
                else if (equippedType == "item" && !string.IsNullOrEmpty(equippedItem))
                {
                    formattedConditions.Add($"equipped:{equippedItem}");
                }
                else
                {
                    formattedConditions.Add("equipped:");
                }
            }
            else if (condition == "form:" && !string.IsNullOrEmpty(formValue))
            {
                formattedConditions.Add($"form:{formValue}");
            }
            else if (condition == "button:" && !string.IsNullOrEmpty(buttonValue))
            {
                formattedConditions.Add($"button:{buttonValue}");
            }
            else if (condition == "threat:" && !string.IsNullOrEmpty(threatValue))
            {
                formattedConditions.Add($"threat:{threatValue}");
            }
            else if (condition == "spec:" && !string.IsNullOrEmpty(specValue))
            {
                formattedConditions.Add($"spec:{specValue}");
            }
            else if (condition == "talent:" && !string.IsNullOrEmpty(talentValue))
            {
                formattedConditions.Add($"talent:{talentValue}");
            }
            else if (condition == "glyph:" && !string.IsNullOrEmpty(glyphValue))
            {
                formattedConditions.Add($"glyph:{glyphValue}");
            }
            else if (condition == "spell:" && !string.IsNullOrEmpty(spellValue))
            {
                formattedConditions.Add($"spell:{spellValue}");
            }
            else if (condition == "item:" && !string.IsNullOrEmpty(itemValue))
            {
                formattedConditions.Add($"item:{itemValue}");
            }
            else if (condition == "aura:" && !string.IsNullOrEmpty(auraValue))
            {
                formattedConditions.Add($"aura:{auraValue}");
            }
            else if ((condition == "buff:" || condition == "debuff:") && !string.IsNullOrEmpty(buffValue))
            {
                formattedConditions.Add($"{condition}{buffValue}");
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
            // Validate advanced conditionals have required arguments
            validationErrors.Clear();
            
            if (SelectedConditions.Contains("stance:") && string.IsNullOrEmpty(stanceValue))
            {
                validationErrors.Add("Stance condition requires a stance value to be selected.");
            }
            
            if (SelectedConditions.Contains("equipped:"))
            {
                if (equippedType == "slot" && string.IsNullOrEmpty(equippedSlot))
                {
                    validationErrors.Add("Equipped condition requires an equipment slot to be selected.");
                }
                else if (equippedType == "item" && string.IsNullOrEmpty(equippedItem))
                {
                    validationErrors.Add("Equipped condition requires an item name to be specified.");
                }
            }
            
            if (SelectedConditions.Contains("form:") && string.IsNullOrEmpty(formValue))
            {
                validationErrors.Add("Form condition requires a form type to be selected.");
            }
            
            if (SelectedConditions.Contains("button:") && string.IsNullOrEmpty(buttonValue))
            {
                validationErrors.Add("Button condition requires a mouse button to be selected.");
            }
            
            if (SelectedConditions.Contains("threat:") && string.IsNullOrEmpty(threatValue))
            {
                validationErrors.Add("Threat condition requires a threat level to be selected.");
            }
            
            if (SelectedConditions.Contains("spec:") && string.IsNullOrEmpty(specValue))
            {
                validationErrors.Add("Spec condition requires a specialization to be specified.");
            }
            
            if (SelectedConditions.Contains("talent:") && string.IsNullOrEmpty(talentValue))
            {
                validationErrors.Add("Talent condition requires a talent to be specified.");
            }
            
            if (SelectedConditions.Contains("glyph:") && string.IsNullOrEmpty(glyphValue))
            {
                validationErrors.Add("Glyph condition requires a glyph to be specified.");
            }
            
            if (SelectedConditions.Contains("spell:") && string.IsNullOrEmpty(spellValue))
            {
                validationErrors.Add("Spell condition requires a spell to be specified.");
            }
            
            if (SelectedConditions.Contains("item:") && string.IsNullOrEmpty(itemValue))
            {
                validationErrors.Add("Item condition requires an item to be specified.");
            }
            
            if (SelectedConditions.Contains("aura:") && string.IsNullOrEmpty(auraValue))
            {
                validationErrors.Add("Aura condition requires an aura to be specified.");
            }
            
            if ((SelectedConditions.Contains("buff:") || SelectedConditions.Contains("debuff:")) && string.IsNullOrEmpty(buffValue))
            {
                validationErrors.Add("Buff/Debuff condition requires a buff/debuff to be specified.");
            }
            
            if (validationErrors.Count > 0)
            {
                // Show validation errors to the user
                StateHasChanged();
                return;
            }

            Block.Configuration["conditions"] = selectedConditionsString;
            Block.Configuration["modifier"] = selectedModifier;
            Block.Configuration["stance"] = stanceValue;
            Block.Configuration["equippedType"] = equippedType;
            Block.Configuration["equippedSlot"] = equippedSlot;
            Block.Configuration["equippedItem"] = equippedItem;
            Block.Configuration["form"] = formValue;
            Block.Configuration["button"] = buttonValue;
            Block.Configuration["threat"] = threatValue;
            Block.Configuration["spec"] = specValue;
            Block.Configuration["talent"] = talentValue;
            Block.Configuration["glyph"] = glyphValue;
            Block.Configuration["spell"] = spellValue;
            Block.Configuration["item"] = itemValue;
            Block.Configuration["aura"] = auraValue;
            Block.Configuration["buff"] = buffValue;
            Block.DisplayText = GetConditionalPreview();
            
            // Clear any previous validation errors
            validationErrors.Clear();
            StateHasChanged();
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 