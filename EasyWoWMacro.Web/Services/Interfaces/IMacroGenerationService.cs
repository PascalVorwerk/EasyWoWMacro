using EasyWoWMacro.Web.Services.Models;

namespace EasyWoWMacro.Web.Services.Interfaces;

public interface IMacroGenerationService
{
    Task<MacroGenerationResult> GenerateFromDescriptionAsync(MacroGenerationRequest request, CancellationToken cancellationToken = default);
    Task<MacroGenerationResult> ValidateAndOptimizeAsync(string macroText, CancellationToken cancellationToken = default);
    bool IsAvailable { get; }
}