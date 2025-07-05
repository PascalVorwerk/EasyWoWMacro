using Microsoft.AspNetCore.Components;
using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Web.Client.Components.MacroStructure;

public partial class ConditionalStructure : ComponentBase
{
    [Parameter] public Conditional? Conditional { get; set; }
} 