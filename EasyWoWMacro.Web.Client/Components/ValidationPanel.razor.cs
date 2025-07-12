namespace EasyWoWMacro.Web.Client.Components;

public partial class ValidationPanel : ComponentBase
{
    [Parameter] public List<ValidationError> ValidationErrors { get; set; } = [];
    [Parameter] public Macro? ParsedMacro { get; set; }
    [Parameter] public bool ShowStructureView { get; set; } = false;
    [Parameter] public EventCallback<bool> OnToggleView { get; set; }

    private string GetFormattedMacroLength()
    {
        return ParsedMacro?.GetFormattedMacro().Length.ToString(CultureInfo.InvariantCulture) ?? "0";
    }

    private static string GetErrorCssClass(ValidationErrorType errorType)
    {
        return errorType switch
        {
            ValidationErrorType.SyntaxError => "validation-error-syntax",
            ValidationErrorType.StructureError => "validation-error-structure",
            ValidationErrorType.CommandError => "validation-error-command",
            ValidationErrorType.ConditionalError => "validation-error-conditional",
            ValidationErrorType.CharacterLimit => "validation-error-limit",
            _ => "validation-error-general"
        };
    }

    private static string GetErrorIcon(ValidationErrorType errorType)
    {
        return errorType switch
        {
            ValidationErrorType.SyntaxError => "⚠️",
            ValidationErrorType.StructureError => "🏗️",
            ValidationErrorType.CommandError => "⚡",
            ValidationErrorType.ConditionalError => "🔮",
            ValidationErrorType.CharacterLimit => "📏",
            _ => "❌"
        };
    }
}