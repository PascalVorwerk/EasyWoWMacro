using Microsoft.AspNetCore.Components;

namespace EasyWoWMacro.Web.Pages.Components;

public partial class QuickTemplates
{
    [Parameter] public EventCallback<string> OnLoadTemplate { get; set; }
} 