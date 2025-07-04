using System.Text.Json.Serialization;

namespace EasyWoWMacro.Web.Models;

public class MacroBlockModel
{
    public string Type { get; set; } = "";
    public string DisplayText { get; set; } = "";
    public Dictionary<string, string> Configuration { get; set; } = new();
} 