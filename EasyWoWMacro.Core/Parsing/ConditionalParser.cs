using EasyWoWMacro.Core.Models;
using EasyWoWMacro.Core.Parsing.Utilities;

namespace EasyWoWMacro.Core.Parsing;

/// <summary>
/// Handles parsing of conditional expressions in WoW macros
/// </summary>
public static class ConditionalParser
{
    /// <summary>
    /// Parses a conditional text into a list of condition sets
    /// </summary>
    /// <param name="conditionalText">The conditional text to parse</param>
    /// <returns>List of condition sets</returns>
    public static List<ConditionSet> ParseConditionSets(string conditionalText)
    {
        var conditionSets = new List<ConditionSet>();
        
        // Split by semicolons for OR logic (different condition sets within the same bracket)
        var orParts = conditionalText.Split(';', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var orPart in orParts)
        {
            var trimmedOrPart = orPart.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedOrPart))
            {
                var conditionSet = new ConditionSet();
                
                // Split by commas for AND logic (conditions within a set)
                var andParts = trimmedOrPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var andPart in andParts)
                {
                    var trimmedAndPart = andPart.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedAndPart))
                    {
                        var condition = ParseCondition(trimmedAndPart);
                        if (condition != null)
                        {
                            conditionSet.Conditions.Add(condition);
                        }
                    }
                }
                
                // Always add the condition set, even if empty (for empty conditionals like [])
                conditionSets.Add(conditionSet);
            }
        }

        // If the conditional text is empty (like []), create an empty condition set
        if (string.IsNullOrWhiteSpace(conditionalText))
        {
            conditionSets.Add(new ConditionSet());
        }

        return conditionSets;
    }

    /// <summary>
    /// Parses a single condition string into a Condition object
    /// </summary>
    /// <param name="conditionText">The condition text to parse</param>
    /// <returns>A Condition object, or null if invalid</returns>
    public static Condition? ParseCondition(string conditionText)
    {
        var trimmed = conditionText.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return null;

        // Check if it's a key:value format
        var colonIndex = trimmed.IndexOf(':');
        if (colonIndex > 0)
        {
            var key = trimmed.Substring(0, colonIndex).Trim();
            var value = trimmed.Substring(colonIndex + 1).Trim();
            return new Condition { Key = key, Value = value };
        }
        else
        {
            // Just a key without value
            return new Condition { Key = trimmed, Value = null };
        }
    }

    /// <summary>
    /// Parses conditionals and arguments from a text segment
    /// </summary>
    /// <param name="text">The text to parse</param>
    /// <returns>Tuple of conditionals and arguments</returns>
    public static (Conditional? conditionals, List<CommandArgument> arguments) ParseConditionalsAndArguments(string text)
    {
        var conditionals = new Conditional();
        var arguments = new List<CommandArgument>();
        var currentPosition = 0;

        // Parse conditionals first (anything in brackets)
        while (currentPosition < text.Length)
        {
            var bracketStart = text.IndexOf('[', currentPosition);
            if (bracketStart == -1)
                break;

            var bracketEnd = StringUtilities.FindMatchingBracket(text, bracketStart);
            if (bracketEnd == -1)
            {
                // Invalid bracket structure - try to extract what looks like a conditional
                var afterBracket = text.Substring(bracketStart + 1).Trim();
                var spaceIndex = afterBracket.IndexOf(' ');
                
                if (spaceIndex > 0)
                {
                    // There's content after the bracket that might be a conditional
                    var potentialConditional = afterBracket.Substring(0, spaceIndex).Trim();
                    var remainingAfterSpace = afterBracket.Substring(spaceIndex + 1).Trim();
                    
                    // Try to parse the potential conditional
                    var parsedConditionSets = ParseConditionSets(potentialConditional);
                    if (parsedConditionSets.Count > 0)
                    {
                        conditionals.ConditionSets.AddRange(parsedConditionSets);
                        
                        // Add the remaining text as an argument
                        if (!string.IsNullOrWhiteSpace(remainingAfterSpace))
                        {
                            arguments.Add(new CommandArgument { Value = remainingAfterSpace });
                        }
                    }
                    else
                    {
                        // If we can't parse it as a conditional, treat everything as an argument
                        var invalidBracketText = text.Substring(currentPosition).Trim();
                        if (!string.IsNullOrWhiteSpace(invalidBracketText))
                        {
                            arguments.Add(new CommandArgument { Value = invalidBracketText });
                        }
                    }
                }
                else
                {
                    // No space found, treat everything as an argument
                    var invalidBracketText = text.Substring(currentPosition).Trim();
                    if (!string.IsNullOrWhiteSpace(invalidBracketText))
                    {
                        arguments.Add(new CommandArgument { Value = invalidBracketText });
                    }
                }
                break;
            }

            var conditionalText = text.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
            var conditionSets = ParseConditionSets(conditionalText);
            conditionals.ConditionSets.AddRange(conditionSets);

            currentPosition = bracketEnd + 1;
        }

        // Parse remaining arguments (everything after the last bracket)
        var remainingText = text.Substring(currentPosition).Trim();
        if (!string.IsNullOrWhiteSpace(remainingText))
        {
            arguments.Add(new CommandArgument { Value = remainingText });
        }

        // Always return conditionals if we found brackets, even if they were empty
        return conditionals.ConditionSets.Count > 0 ? (conditionals, arguments) : (null, arguments);
    }
} 