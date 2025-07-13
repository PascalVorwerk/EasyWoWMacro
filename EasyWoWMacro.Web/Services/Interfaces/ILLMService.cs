using EasyWoWMacro.Web.Services.Models;

namespace EasyWoWMacro.Web.Services.Interfaces;

public interface ILLMService
{
    Task<LLMResponse> SendRequestAsync(LLMRequest request, CancellationToken cancellationToken = default);
    Task<MacroGenerationResult> GenerateMacroAsync(MacroGenerationRequest request, CancellationToken cancellationToken = default);
    bool IsConfigured { get; }
}