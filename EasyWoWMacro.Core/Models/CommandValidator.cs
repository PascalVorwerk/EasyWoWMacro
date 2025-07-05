
namespace EasyWoWMacro.Core.Models;

/// <summary>
/// Validates WoW macro commands against known valid commands
/// </summary>
public static class CommandValidator
{
    /// <summary>
    /// Validates a command against known valid commands
    /// </summary>
    /// <param name="command">The command to validate</param>
    /// <returns>True if the command is valid</returns>
    public static bool IsValidCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return false;
            
        return WoWMacroConstants.ValidSlashCommands.Any(valid => 
            command.StartsWith(valid, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validates a command line and returns any validation errors
    /// </summary>
    /// <param name="commandLine">The command line to validate</param>
    /// <returns>List of validation errors</returns>
    public static List<string> ValidateCommandLine(CommandLine commandLine)
    {
        var errors = new List<string>();

        if (!IsValidCommand(commandLine.Command))
        {
            errors.Add($"Invalid command: {commandLine.Command}");
        }

        return errors;
    }

    /// <summary>
    /// Gets all valid commands
    /// </summary>
    /// <returns>List of valid commands</returns>
    public static IEnumerable<string> GetValidCommands()
    {
        return WoWMacroConstants.ValidSlashCommands.OrderBy(c => c);
    }
} 