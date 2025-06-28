# EasyWoWMacro

A .NET solution for creating and editing World of Warcraft macros with validation and a user-friendly interface.

## Projects

- **EasyWoWMacro.Core**: Core macro parsing and validation logic
- **EasyWoWMacro.Console**: Console application for testing
- **EasyWoWMacro.Web**: Blazor WebAssembly web interface
- **EasyWoWMacro.Tests**: xUnit test suite

## Features

- **Web Interface**: WoW-themed Blazor WebAssembly application
- **Real-time Validation**: Instant feedback on macro syntax and commands
- **Advanced Parsing**: Support for complex WoW macro conditionals and commands
- **Copy to Clipboard**: One-click copying of formatted macros
- **Character Limit Validation**: Ensures macros stay within WoW's 255 character limit
- **Comprehensive Testing**: Full unit test coverage

## Development

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

```bash
# Run the web application
dotnet run --project EasyWoWMacro.Web

# Run tests
dotnet test EasyWoWMacro.Tests

# Run console application
dotnet run --project EasyWoWMacro.Console
```

## Web Interface

The Blazor WebAssembly application provides:

- **WoW-themed UI**: Dark theme with gold accents inspired by WoWhead
- **Macro Editor**: Real-time editing with syntax highlighting
- **Validation Panel**: Shows parsing structure and validation errors
- **Formatted Output**: Clean, copy-ready macro text
- **Responsive Design**: Works on desktop and mobile devices

## Macro Syntax Support

The parser supports the following WoW macro syntax:

### Directives
- `#showtooltip [spell]` - Shows tooltip for specified spell
- `#show [spell]` - Shows icon for specified spell
- `#hide` - Hides the macro icon
- `#icon [spell]` - Sets the macro icon

### Commands with Conditionals
- `/cast [mod:shift,@focus] Polymorph` - Cast with modifier and target conditions
- `/use [combat] Mana Potion; [nocombat] Water` - Multiple conditional arguments
- `/target [harm] Enemy; [help] Ally` - Target based on unit type
- `/cast [mod:shift;@focus] Polymorph` - OR logic within a single bracket
- `/cast [mod:shift,combat][@focus,harm] Polymorph` - Multiple bracket sets with AND logic

### Conditional Logic
The parser correctly handles WoW's complex conditional logic:
- **Comma-separated conditions** (AND logic): `[mod:shift,@focus]` - both conditions must be true
- **Semicolon-separated conditions** (OR logic within bracket): `[mod:shift;@focus]` - either condition can be true
- **Multiple bracket sets** (OR logic): `[mod:shift][@focus]` - either bracket set can be true
- **Semicolon-separated arguments** (fallback alternatives): `/use [combat] Mana Potion; [nocombat] Water`

### Valid Commands
The system validates against a comprehensive list of WoW commands including:
- **Combat**: `/cast`, `/use`, `/castsequence`, `/stopcasting`
- **Target**: `/target`, `/focus`, `/assist`, `/cleartarget`
- **Pet**: `/petattack`, `/petfollow`, `/petstay`, `/petpassive`
- **Equipment**: `/equip`, `/equipslot`, `/use`
- **Auras**: `/cancelaura`, `/cancelform`, `/cancelbuff`
- **Movement**: `/mount`, `/dismount`, `/sit`, `/stand`
- **Communication**: `/say`, `/yell`, `/whisper`, `/party`, `/raid`
- **UI**: `/macro`, `/script`, `/reload`, `/logout`
- **And many more...**

### Valid Conditionals
The system validates against a comprehensive list of WoW conditionals including:
- **Modifiers**: `mod:alt`, `mod:ctrl`, `mod:shift`
- **Targets**: `@target`, `@mouseover`, `@focus`, `@player`, `@cursor`
- **Combat**: `combat`, `nocombat`, `harm`, `help`
- **State**: `mounted`, `flying`, `stealth`, `form`, `stance`
- **Group**: `party`, `raid`, `solo`
- **Equipment**: `equipped`, `worn`
- **And many more...**

## Deployment

### GitHub Pages

The application is automatically deployed to GitHub Pages on push to the `main` branch.

**Live Site**: `https://yourusername.github.io/EasyWoWMacro/`

### GitHub Actions

Two automated workflows are configured:

1. **Build and Test** (`build-and-test.yml`):
   - Runs on every push and pull request
   - Builds the solution with .NET 10.0
   - Runs all unit tests with coverage reporting
   - Posts test results to pull requests

2. **Deploy** (`deploy.yml`):
   - Deploys the Blazor WASM app to GitHub Pages
   - Publishes to the `gh-pages` branch
   - Available at the GitHub Pages URL

### Custom Domain Setup

To use a custom domain with GitHub Pages:

1. **Update the CNAME file**: Edit `EasyWoWMacro.Web/wwwroot/CNAME` and replace `yourdomain.com` with your actual domain.

2. **Configure DNS records** with your domain provider:
   - **A records**: Point your domain to GitHub Pages IP addresses:
     ```
     185.199.108.153
     185.199.109.153
     185.199.110.153
     185.199.111.153
     ```
   - **CNAME record**: Point `www.yourdomain.com` to `yourusername.github.io`

3. **Enable custom domain in GitHub**:
   - Go to your repository Settings → Pages
   - Under "Custom domain", enter your domain
   - Check "Enforce HTTPS" (recommended)
   - Save the settings

4. **Wait for DNS propagation** (can take up to 24 hours)

## Testing

The solution includes comprehensive unit tests covering:

- Macro parsing
- Command validation
- Conditional validation
- Edge cases and error handling

Run tests with:
```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License. 