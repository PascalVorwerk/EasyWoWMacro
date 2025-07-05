# Blazor WebAssembly Migration Plan

## Overview
This document outlines the plan to migrate EasyWoWMacro from a Blazor Server application to a pure Blazor WebAssembly application. The goal is to completely eliminate server-side rendering and move all components to run on the client, reducing server strain and simplifying the architecture.

## Current Architecture Analysis

### Current Setup
- **Project Type**: Blazor Web App with Server Interactivity
- **Framework**: .NET 9.0
- **Render Mode**: InteractiveServer for interactive components
- **Architecture**: Single web project serving both static and interactive content

### Current Interactive Components
1. **MacroEditor.razor** - Main interactive editor with drag-and-drop functionality
2. **BuildingBlocksToolbox.razor** - Interactive toolbox for macro building blocks
3. **MacroLine.razor** - Interactive macro line editor
4. **Configuration Modals** - Various modal dialogs for configuration

### Current Components (All to be moved to WASM)
1. **Home.razor** - Landing page
2. **MacroEditor.razor** - Main interactive editor
3. **Layout components** - Navigation, footer, MainLayout
4. **BuildingBlocks components** - All interactive macro building components

## Migration Strategy

### Option 1: Add Client Project (Recommended)
**Decision**: Add a new client project rather than starting from a new template.

**Rationale**:
- Preserves existing code and structure
- Minimal disruption to current functionality
- Easier to migrate incrementally
- Maintains existing deployment pipeline

### Project Structure After Migration
```
EasyWoWMacro/
├── EasyWoWMacro.Core/           # Shared business logic (unchanged)
├── EasyWoWMacro.Console/        # Console application (unchanged)
├── EasyWoWMacro.Tests/          # Test project (unchanged)
├── EasyWoWMacro.Web/            # Server project (WASM hosting)
│   ├── Components/
│   │   ├── App.razor           # WASM host page with proper structure
│   │   ├── Routes.razor        # Router with additional assemblies
│   │   └── _Imports.razor      # Global imports
│   ├── Program.cs              # WASM hosting setup
│   ├── wwwroot/                # Static assets
│   └── EasyWoWMacro.Web.csproj # Updated with WASM server package
└── EasyWoWMacro.Client/        # WASM client project (main application)
    ├── Pages/
    │   ├── Home.razor          # Landing page
    │   └── MacroEditor.razor   # Interactive editor
    ├── Components/
    │   ├── Layout/             # MainLayout, NavMenu, Footer
    │   └── BuildingBlocks/     # All interactive components
    ├── Services/
    │   └── DragDropService.cs  # Client-side services
    ├── _Imports.razor          # Client-specific imports
    └── Program.cs              # WASM entry point
```

## Implementation Plan

### Phase 1: Create Client Project
1. **Create EasyWoWMacro.Client project**
   - Use `Microsoft.NET.Sdk.BlazorWebAssembly` SDK
   - Target .NET 9.0
   - Reference EasyWoWMacro.Core project

2. **Configure Client Project**
   ```xml
   <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
     <PropertyGroup>
       <TargetFramework>net9.0</TargetFramework>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
       <NoDefaultLaunchSettingsFile>true</NoDefaultLaunchSettingsFile>
       <StaticWebAssetProjectMode>Default</StaticWebAssetProjectMode>
     </PropertyGroup>
     
     <ItemGroup>
       <ProjectReference Include="..\EasyWoWMacro.Core\EasyWoWMacro.Core.csproj" />
       <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
     </ItemGroup>
   </Project>
   ```

3. **Add Client Program.cs**
   ```csharp
   using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
   
   var builder = WebAssemblyHostBuilder.CreateDefault(args);
   
   await builder.Build().RunAsync();
   ```

### Phase 2: Migrate Interactive Components
1. **Move BuildingBlocks components to client project**
   - BuildingBlocksToolbox.razor
   - MacroLine.razor
   - All configuration modals
   - BuildingBlock.razor

2. **Update component render modes**
   - Change from `@rendermode InteractiveServer` to `@rendermode InteractiveWebAssembly`
   - Update any server-specific dependencies

3. **Handle client-side services**
   - Move DragDropService to client project
   - Ensure all dependencies are available in WASM context

### Phase 3: Simplify Server Project
1. **Minimize EasyWoWMacro.Web project**
   - Remove all pages and components
   - Keep only basic WASM hosting setup
   - Move all static assets to wwwroot

2. **Update Program.cs for WASM hosting (correct .NET 9 configuration)**
   ```csharp
   using EasyWoWMacro.Client.Pages;
   using EasyWoWMacro.Components;
   
   var builder = WebApplication.CreateBuilder(args);
   
   // Add services to the container.
   builder.Services.AddRazorComponents()
       .AddInteractiveWebAssemblyComponents();
   
   var app = builder.Build();
   
   // Configure the HTTP request pipeline.
   if (app.Environment.IsDevelopment())
   {
       app.UseWebAssemblyDebugging();
   }
   else
   {
       app.UseExceptionHandler("/Error", createScopeForErrors: true);
       app.UseHsts();
   }
   
   app.UseHttpsRedirection();
   app.UseAntiforgery();
   
   app.MapStaticAssets();
   app.MapRazorComponents<App>()
       .AddInteractiveWebAssemblyRenderMode()
       .AddAdditionalAssemblies(typeof(EasyWoWMacro.Client._Imports).Assembly);
   
   app.Run();
   ```

3. **Update App.razor and Routes.razor**
   - Move App.razor to Components/ folder
   - Update Routes.razor to include additional assemblies
   - Use proper asset references with @Assets syntax
   - Include ImportMap for WASM modules

### Phase 4: Move All Components to Client
1. **Move all pages to client project**
   - Home.razor → EasyWoWMacro.Client/Pages/
   - MacroEditor.razor → EasyWoWMacro.Client/Pages/
   - Remove render mode attributes (pure WASM)

2. **Move all components to client project**
   - Layout components → EasyWoWMacro.Client/Components/Layout/
   - BuildingBlocks components → EasyWoWMacro.Client/Components/BuildingBlocks/
   - Services → EasyWoWMacro.Client/Services/

3. **Update component references and namespaces**
   - Update all using statements to reference client namespaces
   - Ensure proper namespace references in _Imports.razor files
   - Remove any server-specific dependencies
   - Update Routes.razor to include client assembly

### Phase 5: Testing and Validation
1. **Test interactive functionality**
   - Drag and drop operations
   - Modal dialogs
   - Macro validation
   - Clipboard operations

2. **Performance testing**
   - Initial load times
   - Interactive response times
   - Memory usage

## Deployment Changes

### Infrastructure Updates
1. **Update Bicep template**
   - Ensure proper static file serving configuration
   - Add MIME type support for WASM files
   - Configure proper caching headers

2. **App Service Configuration**
   ```json
   {
     "name": "WEBSITE_RUN_FROM_PACKAGE",
     "value": "1"
   },
   {
     "name": "DOTNET_ENVIRONMENT", 
     "value": "Production"
   },
   {
     "name": "ASPNETCORE_ENVIRONMENT",
     "value": "Production"
   }
   ```

3. **Static File Configuration**
   - Ensure `.wasm` files are served with correct MIME type
   - Configure proper caching for WASM assets
   - Enable compression for WASM files

### Build Process Updates
1. **Update solution file**
   - Add EasyWoWMacro.Client project
   - Configure build dependencies

2. **Update server project configuration**
   - Add Microsoft.AspNetCore.Components.WebAssembly.Server package
   - Reference EasyWoWMacro.Client project

3. **Publishing configuration**
   - Ensure client project is built and published
   - Configure proper asset copying
   - Update deployment scripts if needed

## Risk Assessment

### High Risk
- **JavaScript Interop**: Clipboard operations and drag-and-drop may need updates
- **Performance**: Initial WASM download size and startup time
- **Browser Compatibility**: WASM support in older browsers

### Medium Risk
- **Data Serialization**: Complex object serialization between server and client
- **State Management**: Maintaining state across server/client boundary
- **Error Handling**: Different error handling patterns in WASM

### Low Risk
- **Core Logic**: Business logic in EasyWoWMacro.Core
- **Testing**: Existing test suite should remain valid
- **Static Assets**: CSS, JS, and other static files

## Rollback Plan
1. **Keep current server implementation as backup**
2. **Feature flags for gradual rollout**
3. **Monitor performance metrics closely**
4. **Quick rollback to server-only if issues arise**

## Success Criteria
1. **Functionality**: All features work as before in pure WASM environment
2. **Performance**: Reduced server load with client-side execution
3. **User Experience**: No degradation in user experience
4. **Deployment**: Successful deployment with proper WASM asset serving
5. **Architecture**: Clean separation with minimal server footprint

## Timeline Estimate
- **Phase 1-2**: 2-3 days (Client project setup and component migration)
- **Phase 3-4**: 1-2 days (Server simplification and component moves)
- **Phase 5**: 1-2 days (Testing and validation)
- **Total**: 4-7 days

## Next Steps
1. Create EasyWoWMacro.Client project
2. Move all pages and components to client project
3. Simplify server project to minimal WASM hosting
4. Set up development environment for testing
5. Create feature branch for implementation
6. Begin incremental testing of migrated components 