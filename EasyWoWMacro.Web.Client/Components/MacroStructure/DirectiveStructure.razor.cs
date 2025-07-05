using EasyWoWMacro.Core.Models;
using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Client.Components.MacroStructure;

public partial class DirectiveStructure : ComponentBase
{
    [Parameter, EditorRequired]
    public DirectiveLine Directive { get; set; } = null!;
} 