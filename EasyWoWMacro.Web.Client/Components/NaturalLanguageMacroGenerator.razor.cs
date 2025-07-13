using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EasyWoWMacro.Web.Client.Components;

public partial class NaturalLanguageMacroGenerator : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = null!;
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    [Parameter]
    public EventCallback<string> OnMacroGenerated { get; set; }

    private string _description = "";
    private bool _isGenerating = false;
    private bool _hasError = false;
    private string _errorMessage = "";
    private MacroGenerationResult? _lastResult;
    private bool _showCopyToast = false;

    private async Task GenerateMacro()
    {
        if (string.IsNullOrWhiteSpace(_description))
            return;

        _isGenerating = true;
        _hasError = false;
        _errorMessage = "";
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
                _lastResult = result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _hasError = true;
                _errorMessage = $"Generation failed: {response.StatusCode}";

                try
                {
                    var errorResult = JsonSerializer.Deserialize<MacroGenerationResult>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _lastResult = errorResult;
                }
                catch
                {
                    _lastResult = new MacroGenerationResult
                    {
                        Success = false,
                        Errors = new List<string> { errorContent }
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _hasError = true;
            _errorMessage = $"Network error: {ex.Message}";
            _lastResult = new MacroGenerationResult
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

    private async Task UseGeneratedMacro()
    {
        if (_lastResult?.Success == true && !string.IsNullOrEmpty(_lastResult.GeneratedMacro))
        {
            await OnMacroGenerated.InvokeAsync(_lastResult.GeneratedMacro);
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
                _hasError = true;
                _errorMessage = $"Failed to copy: {ex.Message}";
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
