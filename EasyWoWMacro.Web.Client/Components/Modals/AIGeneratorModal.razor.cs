using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.Web.Client.Components.Modals;

public partial class AIGeneratorModal : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<string> OnMacroGenerated { get; set; }
    [Parameter] public EventCallback OnHide { get; set; }

    private string _activeTab = "generate";
    
    // Generate tab state
    private string _description = "";
    private bool _isGenerating = false;
    private bool _hasGenerateError = false;
    private string _generateErrorMessage = "";

    // Fix tab state
    private string _macroToFix = "";
    private bool _isFixing = false;
    private bool _hasFixError = false;
    private string _fixErrorMessage = "";

    // Shared state
    private MacroGenerationResult? _currentResult;
    private bool _showCopyToast = false;

    private void SetActiveTab(string tab)
    {
        _activeTab = tab;
        ClearErrors();
        StateHasChanged();
    }

    public void Show()
    {
        IsVisible = true;
        _activeTab = "generate";
        ClearAll();
        StateHasChanged();
    }

    public async Task Hide()
    {
        IsVisible = false;
        await OnHide.InvokeAsync();
        StateHasChanged();
    }

    private void ClearAll()
    {
        _description = "";
        _macroToFix = "";
        _currentResult = null;
        ClearErrors();
    }

    private void ClearErrors()
    {
        _hasGenerateError = false;
        _generateErrorMessage = "";
        _hasFixError = false;
        _fixErrorMessage = "";
    }

    private async Task GenerateMacro()
    {
        if (string.IsNullOrWhiteSpace(_description))
            return;

        _isGenerating = true;
        _hasGenerateError = false;
        _currentResult = null;
        StateHasChanged();

        try
        {
            var request = new MacroGenerationRequest
            {
                Description = _description.Trim()
            };

            var response = await Http.PostAsJsonAsync("/api/MacroGeneration/generate", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MacroGenerationResult>();
                _currentResult = result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _hasGenerateError = true;
                _generateErrorMessage = $"Generation failed: {response.StatusCode}";
                
                try
                {
                    var errorResult = JsonSerializer.Deserialize<MacroGenerationResult>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _currentResult = errorResult;
                }
                catch
                {
                    _currentResult = new MacroGenerationResult
                    {
                        Success = false,
                        Errors = new List<string> { errorContent }
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _hasGenerateError = true;
            _generateErrorMessage = $"Network error: {ex.Message}";
            _currentResult = new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
        finally
        {
            _isGenerating = false;
            StateHasChanged();
        }
    }

    private async Task FixMacro()
    {
        if (string.IsNullOrWhiteSpace(_macroToFix))
            return;

        _isFixing = true;
        _hasFixError = false;
        _currentResult = null;
        StateHasChanged();

        try
        {
            // For now, use the validate endpoint to check the macro
            // We'll enhance this with proper fix functionality in the next step
            var request = new ValidateMacroRequest
            {
                MacroText = _macroToFix.Trim()
            };

            var response = await Http.PostAsJsonAsync("/api/MacroGeneration/validate", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MacroGenerationResult>();
                
                if (result?.Success == true)
                {
                    // Macro is already valid
                    _currentResult = new MacroGenerationResult
                    {
                        Success = true,
                        GeneratedMacro = _macroToFix.Trim(),
                        Explanation = "Your macro is already valid! No fixes needed.",
                        CharacterCount = _macroToFix.Trim().Length,
                        ProcessingTime = TimeSpan.FromMilliseconds(100)
                    };
                }
                else
                {
                    // For now, just show the validation errors
                    // TODO: Implement actual fixing logic
                    _currentResult = new MacroGenerationResult
                    {
                        Success = false,
                        GeneratedMacro = _macroToFix.Trim(),
                        Errors = result?.Errors ?? new List<string> { "Macro validation failed" },
                        Explanation = "Fix functionality is coming soon! For now, here are the issues found with your macro."
                    };
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _hasFixError = true;
                _fixErrorMessage = $"Validation failed: {response.StatusCode}";
                
                _currentResult = new MacroGenerationResult
                {
                    Success = false,
                    Errors = new List<string> { errorContent }
                };
            }
        }
        catch (Exception ex)
        {
            _hasFixError = true;
            _fixErrorMessage = $"Network error: {ex.Message}";
            _currentResult = new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
        finally
        {
            _isFixing = false;
            StateHasChanged();
        }
    }

    private async Task UseGeneratedMacro()
    {
        if (_currentResult?.Success == true && !string.IsNullOrEmpty(_currentResult.GeneratedMacro))
        {
            await OnMacroGenerated.InvokeAsync(_currentResult.GeneratedMacro);
            await Hide();
        }
    }

    private async Task CopyToClipboard(string text)
    {
        try
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", text);
            await ShowCopyToast();
        }
        catch
        {
            // Fallback for older browsers
            try
            {
                await JS.InvokeVoidAsync("copyToClipboard", text);
                await ShowCopyToast();
            }
            catch (Exception ex)
            {
                if (_activeTab == "generate")
                {
                    _hasGenerateError = true;
                    _generateErrorMessage = $"Failed to copy: {ex.Message}";
                }
                else
                {
                    _hasFixError = true;
                    _fixErrorMessage = $"Failed to copy: {ex.Message}";
                }
                StateHasChanged();
            }
        }
    }

    private async Task ShowCopyToast()
    {
        _showCopyToast = true;
        StateHasChanged();

        await Task.Delay(3000);
        _showCopyToast = false;
        StateHasChanged();
    }
}

// DTO classes for API communication
public class MacroGenerationRequest
{
    public string Description { get; set; } = string.Empty;
    public bool IsPremiumUser { get; set; } = false;
}

public class ValidateMacroRequest
{
    public string MacroText { get; set; } = string.Empty;
}

public class MacroGenerationResult
{
    public bool Success { get; set; }
    public string? GeneratedMacro { get; set; }
    public string? Explanation { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public int CharacterCount { get; set; }
    public int TokensUsed { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}