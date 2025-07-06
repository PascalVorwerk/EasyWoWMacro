namespace EasyWoWMacro.Core.Parsing.Utilities;

/// <summary>
/// Utility methods for string operations used in macro parsing
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Splits a string by semicolons, but only when outside of brackets
    /// </summary>
    /// <param name="input">The input string to split</param>
    /// <returns>List of parts split by semicolons outside brackets</returns>
    public static List<string> SplitBySemicolonsOutsideBrackets(string input)
    {
        var result = new List<string>();
        int bracketDepth = 0;
        int lastSplit = 0;
        
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '[') 
                bracketDepth++;
            else if (input[i] == ']') 
                bracketDepth--;
            else if (input[i] == ';' && bracketDepth == 0)
            {
                result.Add(input.Substring(lastSplit, i - lastSplit));
                lastSplit = i + 1;
            }
        }
        
        if (lastSplit < input.Length)
            result.Add(input.Substring(lastSplit));
            
        return result;
    }

    /// <summary>
    /// Finds the matching closing bracket for an opening bracket
    /// </summary>
    /// <param name="text">The text to search in</param>
    /// <param name="startIndex">The index of the opening bracket</param>
    /// <returns>The index of the matching closing bracket, or -1 if not found</returns>
    public static int FindMatchingBracket(string text, int startIndex)
    {
        var bracketCount = 0;
        for (int i = startIndex; i < text.Length; i++)
        {
            if (text[i] == '[')
                bracketCount++;
            else if (text[i] == ']')
            {
                bracketCount--;
                if (bracketCount == 0)
                    return i;
            }
        }
        return -1; // No matching bracket found
    }

    /// <summary>
    /// Tokenizes a command line by spaces, keeping brackets together as single tokens
    /// </summary>
    /// <param name="line">The command line to tokenize</param>
    /// <returns>List of tokens</returns>
    public static List<string> TokenizeCommandLine(string line)
    {
        var tokens = new List<string>();
        int idx = 0;
        
        while (idx < line.Length)
        {
            if (line[idx] == '[')
            {
                int end = line.IndexOf(']', idx);
                if (end != -1)
                {
                    tokens.Add(line.Substring(idx, end - idx + 1));
                    idx = end + 1;
                }
                else
                {
                    tokens.Add(line.Substring(idx));
                    break;
                }
            }
            else if (line[idx] == ' ')
            {
                idx++;
            }
            else
            {
                int nextSpace = line.IndexOf(' ', idx);
                if (nextSpace == -1)
                {
                    tokens.Add(line.Substring(idx));
                    break;
                }
                else
                {
                    tokens.Add(line.Substring(idx, nextSpace - idx));
                    idx = nextSpace;
                }
            }
        }
        
        return tokens;
    }

    /// <summary>
    /// Checks if a string starts with any of the known conditionals
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if the text starts with a known conditional</returns>
    public static bool StartsWithKnownConditional(string text)
    {
        var knownConditionals = Models.ConditionalValidator.GetValidConditionalKeys();
        
        foreach (var cond in knownConditionals)
        {
            if (text.StartsWith(cond + ",") || text.StartsWith(cond + " ") || text == cond)
                return true;
        }
        
        return false;
    }
} 