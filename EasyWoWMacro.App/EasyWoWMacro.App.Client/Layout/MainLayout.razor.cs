﻿using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace EasyWoWMacro.App.Client.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public AccentBaseColor AccentBaseColor { get; set; } = default!;
    [Inject] public NeutralBaseColor NeutralBaseColor { get; set; } = default!;
    [Inject] public BaseLayerLuminance BaseLayerLuminance { get; set; } = default!;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Set to dark mode by default
            await BaseLayerLuminance.WithDefault((float)0.15);
            
            
        }
    }
}