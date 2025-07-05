using EasyWoWMacro.Core.Models;
using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Client.Components.MacroStructure;

public partial class CommentStructure : ComponentBase
{
    [Parameter, EditorRequired]
    public CommentLine Comment { get; set; } = null!;
} 