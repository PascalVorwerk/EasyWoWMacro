namespace EasyWoWMacro.Web.Client.Models;

/// <summary>
/// Configuration metadata for advanced conditionals
/// </summary>
public class ConditionalConfigurationInfo
{
    public string Conditional { get; set; } = string.Empty;
    public string ConfigurationKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public ConfigurationType Type { get; set; }
    public string? Description { get; set; }
    public List<SelectOption>? Options { get; set; }
}

/// <summary>
/// Type of configuration input
/// </summary>
public enum ConfigurationType
{
    Text,
    Select,
    EquipmentSlot,
    EquipmentItem
}

/// <summary>
/// Option for select inputs
/// </summary>
public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Group { get; set; }
} 