using EasyWoWMacro.Web.Services.Interfaces;
using EasyWoWMacro.Web.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyWoWMacro.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MacroGenerationController : ControllerBase
{
    private readonly IMacroGenerationService _macroGenerationService;
    private readonly ILogger<MacroGenerationController> _logger;

    public MacroGenerationController(
        IMacroGenerationService macroGenerationService,
        ILogger<MacroGenerationController> logger)
    {
        _macroGenerationService = macroGenerationService;
        _logger = logger;
    }

    /// <summary>
    /// Get the status of the macro generation service
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            Available = _macroGenerationService.IsAvailable,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Generate a WoW macro from a natural language description
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMacro([FromBody] MacroGenerationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest(new { Error = "Description is required" });
        }

        if (request.Description.Length > 500)
        {
            return BadRequest(new { Error = "Description must be 500 characters or less" });
        }

        try
        {
            _logger.LogInformation("Generating macro for description: {Description}", request.Description);
            
            var result = await _macroGenerationService.GenerateFromDescriptionAsync(request, cancellationToken);
            
            _logger.LogInformation("Macro generation completed: {Success}, Processing time: {ProcessingTime}ms", 
                result.Success, result.ProcessingTime.TotalMilliseconds);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating macro for description: {Description}", request.Description);
            
            return StatusCode(500, new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { "An unexpected error occurred while generating the macro" }
            });
        }
    }

    /// <summary>
    /// Validate and optimize an existing macro
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateMacro([FromBody] ValidateMacroRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.MacroText))
        {
            return BadRequest(new { Error = "MacroText is required" });
        }

        if (request.MacroText.Length > 500)
        {
            return BadRequest(new { Error = "Macro text must be 500 characters or less" });
        }

        try
        {
            _logger.LogDebug("Validating macro: {MacroText}", request.MacroText);
            
            var result = await _macroGenerationService.ValidateAndOptimizeAsync(request.MacroText, cancellationToken);
            
            _logger.LogDebug("Macro validation completed: {Success}", result.Success);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating macro: {MacroText}", request.MacroText);
            
            return StatusCode(500, new MacroGenerationResult
            {
                Success = false,
                Errors = new List<string> { "An unexpected error occurred while validating the macro" }
            });
        }
    }
}

public record ValidateMacroRequest
{
    public string MacroText { get; init; } = string.Empty;
}