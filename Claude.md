# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

EasyWoWMacro is a .NET 9 solution for creating and editing World of Warcraft macros with real-time validation and a user-friendly web interface. The project is deployed live at https://pcwvorwerk.github.io/EasyWoWMacro/.

## Development Commands

### Building and Running
```bash
# Build the entire solution
dotnet build

# Run the web application (main entry point)
dotnet run --project EasyWoWMacro.Web

# Run the console application (for testing)
dotnet run --project EasyWoWMacro.Console

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a specific test file
dotnet test EasyWoWMacro.Tests --filter ClassName=MacroParserTests
```

### Development URLs
- **HTTPS**: https://localhost:7158
- **HTTP**: http://localhost:5180

## Architecture Overview

### Core Domain Model
The solution follows a layered architecture with clear separation of concerns:

- **MacroLine**: Abstract base class for all macro line types (CommandLine, DirectiveLine, CommentLine)
- **Macro**: Root aggregate containing a collection of MacroLine objects with formatting and validation
- **CommandLine**: Represents WoW commands with complex conditional logic via CommandClauses
- **Conditional**: Handles WoW's sophisticated conditional system with ConditionSets (OR logic) and Conditions (AND logic)
- **MacroParser**: Core parsing engine that converts raw text into structured domain objects

### WoW Macro Conditional Logic
The system correctly implements WoW's complex conditional syntax:
- **[mod:shift,combat]** - Comma-separated conditions (AND logic)
- **[mod:shift;combat]** - Semicolon-separated conditions (OR logic within brackets)
- **[mod:shift][@target]** - Multiple bracket sets (OR logic between sets)
- **command [cond1] arg1; [cond2] arg2; fallback** - Semicolon-separated clauses (fallback chain)

### Web Architecture
The web layer uses Blazor WebAssembly with a host/client split:
- **EasyWoWMacro.Web**: Server-side host project
- **EasyWoWMacro.Web.Client**: Client-side WebAssembly project with UI components

### Key Components
- **MacroEditor**: Main editing interface with visual block-based editing and import/export
- **MacroStructureViewer**: Displays parsed macro structure for debugging
- **Building Blocks**: Drag-and-drop components for visual macro construction
- **ConditionalService**: Manages conditional editing and validation

## Testing Strategy

The solution uses xUnit for testing with comprehensive coverage:
- **MacroParserTests**: Core parsing logic validation
- **CommandValidatorTests**: Command validation logic
- **ConditionalValidatorTests**: Conditional parsing and validation

## Validation System

Multi-layered validation approach:
1. **Syntax Validation**: Bracket matching, character limits (255 chars for WoW)
2. **Command Validation**: Against WoW's extensive command list in Constants.cs
3. **Conditional Validation**: Against WoW's conditional system
4. **Semantic Validation**: Cross-referencing and logical consistency

## Important Development Notes

### WoW Macro Constraints
- Maximum 255 characters for the final formatted macro
- Specific command syntax requirements
- Complex conditional logic with AND/OR combinations
- Extensive list of valid commands and conditionals maintained in Constants.cs

### WoW Macro Reference Documentation
- **Official WoW Macro Guide**: https://wowpedia.fandom.com/wiki/Making_a_macro
  - Complete EBNF syntax for macro structure
  - Comprehensive documentation on macro creation and syntax rules
  - Reference for all valid commands and conditionals
- **WoW Macro Conditionals**: https://wowpedia.fandom.com/wiki/Macro_conditionals
  - Complete reference for all available macro conditionals
  - Detailed syntax and usage examples for conditional logic

### Blazor Development Guidelines
- Uses .NET 9 Blazor WebAssembly hosted model
- Code-behind files (.razor.cs) are preferred over inline code
- Components are split into logical, reusable pieces
- State management uses services and component parameters

### Key Files to Understand
- `EasyWoWMacro.Core/Models/`: Domain models representing macro structure
- `EasyWoWMacro.Core/Parsing/MacroParser.cs`: Core parsing logic
- `EasyWoWMacro.Core/Models/Constants.cs`: WoW command and conditional definitions
- `EasyWoWMacro.Web.Client/Pages/MacroEditor.razor.cs`: Main UI logic

## Azure Deployment

The project includes infrastructure as code using Azure Bicep:
- **main.bicep**: Defines App Service Plan and Web App
- **parameters.json**: Environment-specific configuration
- Configured for .NET 9 runtime on Linux

## GitHub Pages Deployment

The application is automatically deployed to GitHub Pages on push to main branch at:
https://pcwvorwerk.github.io/EasyWoWMacro/

## Development Environment

- **Target Framework**: .NET 9
- **IDE**: Optimized for Visual Studio 2022 or JetBrains Rider
- **Testing**: xUnit with code coverage
- **Frontend**: Blazor WebAssembly with Bootstrap styling and custom WoW theme