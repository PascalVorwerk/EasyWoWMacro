using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Web.Client.Models;

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

    private string _selectedConditionsString = "";
    private string _selectedModifier = "";
    private string _stanceValue = "";
    private string _equippedType = "slot";
    private string _equippedSlot = "";
    private string _equippedItem = "";
    private string _formValue = "";
    private string _buttonValue = "";
    private string _threatValue = "";
    private string _specValue = "";
    private string _talentValue = "";
    private string _glyphValue = "";
    private string _spellValue = "";
    private string _itemValue = "";
    private string _auraValue = "";
    private string _buffValue = "";
    private readonly List<string> _validationErrors = [];

    private readonly string[] _basicConditions =
    [
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
    ];

    private readonly string[] _advancedConditions =
    [
        "mod:", "nomod:", "button:", "form:", "equipped:", "worn:", "threat:", 
        "@target", "@mouseover", "@focus", "@cursor", "@player", "@npc", "@pet",
        "spec:", "talent:", "glyph:", "spell:", "item:", "aura:", "buff:", "debuff:"
    ];

    private List<string> SelectedConditions => 
        _selectedConditionsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

    protected override void OnParametersSet()
    {
        if (Block != null && Block.Configuration.TryGetValue("conditions", out var conditions))
        {
            _selectedConditionsString = conditions;
        }
        if (Block != null && Block.Configuration.TryGetValue("modifier", out var modifier))
        {
            _selectedModifier = modifier;
        }
        if (Block != null && Block.Configuration.TryGetValue("stance", out var stance))
        {
            _stanceValue = stance;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedType", out var eqType))
        {
            _equippedType = eqType;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedSlot", out var eqSlot))
        {
            _equippedSlot = eqSlot;
        }
        if (Block != null && Block.Configuration.TryGetValue("equippedItem", out var eqItem))
        {
            _equippedItem = eqItem;
        }
        if (Block != null && Block.Configuration.TryGetValue("form", out var form))
        {
            _formValue = form;
        }
        if (Block != null && Block.Configuration.TryGetValue("button", out var button))
        {
            _buttonValue = button;
        }
        if (Block != null && Block.Configuration.TryGetValue("threat", out var threat))
        {
            _threatValue = threat;
        }
        if (Block != null && Block.Configuration.TryGetValue("spec", out var spec))
        {
            _specValue = spec;
        }
        if (Block != null && Block.Configuration.TryGetValue("talent", out var talent))
        {
            _talentValue = talent;
        }
        if (Block != null && Block.Configuration.TryGetValue("glyph", out var glyph))
        {
            _glyphValue = glyph;
        }
        if (Block != null && Block.Configuration.TryGetValue("spell", out var spell))
        {
            _spellValue = spell;
        }
        if (Block != null && Block.Configuration.TryGetValue("item", out var item))
        {
            _itemValue = item;
        }
        if (Block != null && Block.Configuration.TryGetValue("aura", out var aura))
        {
            _auraValue = aura;
        }
        if (Block != null && Block.Configuration.TryGetValue("buff", out var buff))
        {
            _buffValue = buff;
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
        
        _selectedConditionsString = string.Join(",", currentList);
    }

    private string GetConditionalPreview()
    {
        var conditions = SelectedConditions;
        if (conditions.Count == 0) return "[No conditions selected]";
        
        var formattedConditions = new List<string>();
        foreach (var condition in conditions)
        {
            if (condition == "mod:" && !string.IsNullOrEmpty(_selectedModifier))
            {
                formattedConditions.Add($"mod:{_selectedModifier}");
            }
            else if (condition == "nomod:" && !string.IsNullOrEmpty(_selectedModifier))
            {
                formattedConditions.Add($"nomod:{_selectedModifier}");
            }
            else if (condition == "stance:" && !string.IsNullOrEmpty(_stanceValue))
            {
                formattedConditions.Add($"stance:{_stanceValue}");
            }
            else if (condition == "equipped:" && !string.IsNullOrEmpty(_equippedType))
            {
                if (_equippedType == "slot" && !string.IsNullOrEmpty(_equippedSlot))
                {
                    formattedConditions.Add($"equipped:{_equippedSlot}");
                }
                else if (_equippedType == "item" && !string.IsNullOrEmpty(_equippedItem))
                {
                    formattedConditions.Add($"equipped:{_equippedItem}");
                }
                else
                {
                    formattedConditions.Add("equipped:");
                }
            }
            else if (condition == "form:" && !string.IsNullOrEmpty(_formValue))
            {
                formattedConditions.Add($"form:{_formValue}");
            }
            else if (condition == "button:" && !string.IsNullOrEmpty(_buttonValue))
            {
                formattedConditions.Add($"button:{_buttonValue}");
            }
            else if (condition == "threat:" && !string.IsNullOrEmpty(_threatValue))
            {
                formattedConditions.Add($"threat:{_threatValue}");
            }
            else if (condition == "spec:" && !string.IsNullOrEmpty(_specValue))
            {
                formattedConditions.Add($"spec:{_specValue}");
            }
            else if (condition == "talent:" && !string.IsNullOrEmpty(_talentValue))
            {
                formattedConditions.Add($"talent:{_talentValue}");
            }
            else if (condition == "glyph:" && !string.IsNullOrEmpty(_glyphValue))
            {
                formattedConditions.Add($"glyph:{_glyphValue}");
            }
            else if (condition == "spell:" && !string.IsNullOrEmpty(_spellValue))
            {
                formattedConditions.Add($"spell:{_spellValue}");
            }
            else if (condition == "item:" && !string.IsNullOrEmpty(_itemValue))
            {
                formattedConditions.Add($"item:{_itemValue}");
            }
            else if (condition == "aura:" && !string.IsNullOrEmpty(_auraValue))
            {
                formattedConditions.Add($"aura:{_auraValue}");
            }
            else if ((condition == "buff:" || condition == "debuff:") && !string.IsNullOrEmpty(_buffValue))
            {
                formattedConditions.Add($"{condition}{_buffValue}");
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
            _validationErrors.Clear();
            
            if (SelectedConditions.Contains("stance:") && string.IsNullOrEmpty(_stanceValue))
            {
                _validationErrors.Add("Stance condition requires a stance value to be selected.");
            }
            
            if (SelectedConditions.Contains("equipped:"))
            {
                if (_equippedType == "slot" && string.IsNullOrEmpty(_equippedSlot))
                {
                    _validationErrors.Add("Equipped condition requires an equipment slot to be selected.");
                }
                else if (_equippedType == "item" && string.IsNullOrEmpty(_equippedItem))
                {
                    _validationErrors.Add("Equipped condition requires an item name to be specified.");
                }
            }
            
            if (SelectedConditions.Contains("form:") && string.IsNullOrEmpty(_formValue))
            {
                _validationErrors.Add("Form condition requires a form type to be selected.");
            }
            
            if (SelectedConditions.Contains("button:") && string.IsNullOrEmpty(_buttonValue))
            {
                _validationErrors.Add("Button condition requires a mouse button to be selected.");
            }
            
            if (SelectedConditions.Contains("threat:") && string.IsNullOrEmpty(_threatValue))
            {
                _validationErrors.Add("Threat condition requires a threat level to be selected.");
            }
            
            if (SelectedConditions.Contains("spec:") && string.IsNullOrEmpty(_specValue))
            {
                _validationErrors.Add("Spec condition requires a specialization to be specified.");
            }
            
            if (SelectedConditions.Contains("talent:") && string.IsNullOrEmpty(_talentValue))
            {
                _validationErrors.Add("Talent condition requires a talent to be specified.");
            }
            
            if (SelectedConditions.Contains("glyph:") && string.IsNullOrEmpty(_glyphValue))
            {
                _validationErrors.Add("Glyph condition requires a glyph to be specified.");
            }
            
            if (SelectedConditions.Contains("spell:") && string.IsNullOrEmpty(_spellValue))
            {
                _validationErrors.Add("Spell condition requires a spell to be specified.");
            }
            
            if (SelectedConditions.Contains("item:") && string.IsNullOrEmpty(_itemValue))
            {
                _validationErrors.Add("Item condition requires an item to be specified.");
            }
            
            if (SelectedConditions.Contains("aura:") && string.IsNullOrEmpty(_auraValue))
            {
                _validationErrors.Add("Aura condition requires an aura to be specified.");
            }
            
            if ((SelectedConditions.Contains("buff:") || SelectedConditions.Contains("debuff:")) && string.IsNullOrEmpty(_buffValue))
            {
                _validationErrors.Add("Buff/Debuff condition requires a buff/debuff to be specified.");
            }
            
            if (_validationErrors.Count > 0)
            {
                // Show validation errors to the user
                StateHasChanged();
                return;
            }

            Block.Configuration["conditions"] = _selectedConditionsString;
            Block.Configuration["modifier"] = _selectedModifier;
            Block.Configuration["stance"] = _stanceValue;
            Block.Configuration["equippedType"] = _equippedType;
            Block.Configuration["equippedSlot"] = _equippedSlot;
            Block.Configuration["equippedItem"] = _equippedItem;
            Block.Configuration["form"] = _formValue;
            Block.Configuration["button"] = _buttonValue;
            Block.Configuration["threat"] = _threatValue;
            Block.Configuration["spec"] = _specValue;
            Block.Configuration["talent"] = _talentValue;
            Block.Configuration["glyph"] = _glyphValue;
            Block.Configuration["spell"] = _spellValue;
            Block.Configuration["item"] = _itemValue;
            Block.Configuration["aura"] = _auraValue;
            Block.Configuration["buff"] = _buffValue;
            Block.DisplayText = GetConditionalPreview();
            
            // Clear any previous validation errors
            _validationErrors.Clear();
            StateHasChanged();
            
            await OnSave.InvokeAsync(Block);
        }
    }
} 