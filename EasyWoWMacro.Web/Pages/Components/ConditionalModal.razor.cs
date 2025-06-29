using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class ConditionalModal
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public List<string> SelectedConditionals { get; set; } = new();
    [Parameter] public EventCallback<List<string>> OnSaveCallback { get; set; }
    [Parameter] public EventCallback OnCancelCallback { get; set; }

    private string SearchTerm { get; set; } = "";
    private string ActiveSection { get; set; } = "modifiers";

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    private IEnumerable<string> GetFilteredConditionals(string[] conditionals)
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return conditionals;

        return conditionals.Where(c => c.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsConditionalSelected(string conditional)
    {
        return SelectedConditionals.Contains(conditional);
    }

    private void OnConditionalToggle(string conditional, object? isChecked)
    {
        if (isChecked is bool checkedValue)
        {
            if (checkedValue && !SelectedConditionals.Contains(conditional))
            {
                SelectedConditionals.Add(conditional);
            }
            else if (!checkedValue && SelectedConditionals.Contains(conditional))
            {
                SelectedConditionals.Remove(conditional);
            }
        }
    }

    private void RemoveConditional(string conditional)
    {
        SelectedConditionals.Remove(conditional);
    }

    private string GetAccordionClass(string section)
    {
        return ActiveSection == section ? "" : "collapsed";
    }

    private string GetAccordionShowClass(string section)
    {
        return ActiveSection == section ? "show" : "";
    }

    private async Task OnSave()
    {
        await OnSaveCallback.InvokeAsync(SelectedConditionals);
    }

    private async Task OnCancel()
    {
        await OnCancelCallback.InvokeAsync();
    }
}
