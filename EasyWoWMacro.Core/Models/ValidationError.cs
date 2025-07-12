namespace EasyWoWMacro.Core.Models;

public class ValidationError
{
    public string Message { get; set; } = string.Empty;
    public ValidationErrorType Type { get; set; }
    public string? QuickFix { get; set; }
    public string? GuideSection { get; set; }
    public string? Example { get; set; }
    public string? Explanation { get; set; }
}

public enum ValidationErrorType
{
    SyntaxError,
    StructureError,
    CommandError,
    ConditionalError,
    CharacterLimit,
    GeneralError
}