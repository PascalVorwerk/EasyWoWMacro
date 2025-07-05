using EasyWoWMacro.Web.Client.Models;

namespace EasyWoWMacro.Web.Client.Services;

/// <summary>
/// Service for handling conditional operations using core validation classes
/// </summary>
public interface IConditionalService
{
    /// <summary>
    /// Gets advanced conditions (those that can have values)
    /// </summary>
    IEnumerable<string> GetAdvancedConditions();

    /// <summary>
    /// Gets all available basic conditionals (for search functionality)
    /// </summary>
    IEnumerable<string> GetAllBasicConditions();

    /// <summary>
    /// Gets configuration metadata for advanced conditionals
    /// </summary>
    IEnumerable<ConditionalConfigurationInfo> GetConditionalConfigurationInfo();
}
