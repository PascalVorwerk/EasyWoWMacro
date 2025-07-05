using EasyWoWMacro.Core.Models;
using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Client.Components.MacroStructure;

public partial class CommandStructure : ComponentBase
{
    [Parameter, EditorRequired]
    public CommandLine Command { get; set; } = null!;

    private bool HasClauses => Command.Clauses.Count > 0;
} 