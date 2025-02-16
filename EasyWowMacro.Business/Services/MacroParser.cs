using System.Text;
using EasyWoWMacro.Business.Models;
using EasyWoWMacro.Business.Services.Interface;

namespace EasyWoWMacro.Business.Services
{
    public class MacroParser : IMacroParser
    {
        private string _input = string.Empty;
        private int _position;

        /// <summary>
        /// Entry point – parse the whole macro.
        /// According to our EBNF, a macro consists of a single command.
        /// </summary>
        public Macro ParseMacro(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input must not be null or empty.", nameof(input));

            _input = input;
            _position = 0;

            var macro = new Macro { Value = input };
            SkipWhitespace();
            var command = ParseCommand();
            macro.Commands.Add(command);
            SkipWhitespace();
            if (!IsEnd())
                throw new Exception("Extra characters found after the command.");
            return macro;
        }

        /// <summary>
        /// command = "/" command-verb, [ command-object { ";" command-object } ]
        /// </summary>
        private MacroCommand ParseCommand()
        {
            SkipWhitespace();
            if (!Match("/") && !Match("#showtooltip"))
                throw new Exception("Command must start with '/'.");

            Consume(); // consume '/'

            SkipWhitespace();
            string verb = ParseCommandVerb();
            var command = new MacroCommand { Verb = verb };
            SkipWhitespace();

            // Parse one or more command objects (separated by semicolons)
            if (!IsEnd())
            {
                var cmdObj = ParseCommandObject();
                command.Objects.Add(cmdObj);
                SkipWhitespace();

                while (Match(";"))
                {
                    Consume(); // consume ';'
                    SkipWhitespace();
                    cmdObj = ParseCommandObject();
                    command.Objects.Add(cmdObj);
                    SkipWhitespace();
                }
            }
            return command;
        }

        /// <summary>
        /// command-verb = any secure command word (treated as an identifier).
        /// </summary>
        private string ParseCommandVerb() => ParseIdentifier();

        /// <summary>
        /// command-object = { condition } parameters
        /// Conditions (if present) must come first.
        /// </summary>
        private CommandObject ParseCommandObject()
        {
            var commandObj = new CommandObject();
            SkipWhitespace();
            // While conditions are present, parse them.
            while (!IsEnd() && Peek() == '[')
            {
                // Use helper to get the entire condition text (including matching ']')
                string conditionText = ParseBracketedConditionText();
                // Now, split the condition text on commas and parse each phrase.
                var phrases = conditionText.Split(',')
                                             .Select(p => p.Trim())
                                             .Where(p => !string.IsNullOrEmpty(p))
                                             .Select(p => ParseConditionPhraseFromToken(p))
                                             .ToList();
                var condition = new Condition { Phrases = phrases };
                commandObj.Conditions.Add(condition);
                SkipWhitespace();
            }
            // The remainder (until a semicolon or end-of-input) is taken as parameters.
            StringBuilder sb = new StringBuilder();
            while (!IsEnd() && Peek() != ';')
            {
                sb.Append(Consume());
            }
            commandObj.Parameters = sb.ToString().Trim();
            return commandObj;
        }

        /// <summary>
        /// Helper to parse text inside a condition: it assumes the current position is at '['.
        /// It returns the text between '[' and the matching ']'. Throws if no closing bracket is found.
        /// </summary>
        private string ParseBracketedConditionText()
        {
            SkipWhitespace();
            if (!Match("["))
                throw new Exception("Expected '[' at position " + _position);
            Consume(); // consume '['

            int start = _position;
            while (!IsEnd() && Peek() != ']')
            {
                char c = Peek();
                if (c == '\n')
                    throw new Exception($"Newline encountered in condition starting at position {start}");
                if (c == '[')
                    throw new Exception($"Unexpected '[' found inside condition starting at position {start}");
                // We allow '/' here because it’s valid after a colon.
                Consume();
            }
            
            if (IsEnd())
                throw new Exception($"Unclosed condition starting at position {start}");
            // The condition content is from 'start' up to the current position (which should be at ']').
            string content = _input.Substring(start, _position - start).Trim();
            Consume(); // consume closing ']'
            return content;
        }

        /// <summary>
        /// Parses a single condition phrase from a token (which has already been trimmed).
        /// The token can either start with '@' (target condition) or be an option condition.
        /// </summary>
        private ConditionPhrase ParseConditionPhraseFromToken(string token)
        {
            if (token.StartsWith("@"))
            {
                string target = token.Substring(1).Trim();
                if (string.IsNullOrEmpty(target))
                    throw new Exception("Target condition missing target pattern.");
                return new TargetConditionPhrase { TargetPattern = target };
            }
            else
            {
                bool isNegated = false;
                // Check if token starts with "no" exactly.
                if (token.StartsWith("no"))
                {
                    // Ensure that "no" is a complete token prefix (e.g. "nohelp" means negated help)
                    isNegated = true;
                    token = token.Substring(2).Trim();
                }
                // Now token should be of the form optionWord [ ":" arguments ]
                string optionWord;
                List<string> arguments = new List<string>();

                int colonIndex = token.IndexOf(':');
                if (colonIndex >= 0)
                {
                    optionWord = token.Substring(0, colonIndex).Trim();
                    string argsPart = token.Substring(colonIndex + 1);
                    arguments = argsPart.Split('/')
                                        .Select(arg => arg.Trim())
                                        .Where(arg => !string.IsNullOrEmpty(arg))
                                        .ToList();
                }
                else
                {
                    optionWord = token;
                }
                if (string.IsNullOrEmpty(optionWord))
                    throw new Exception("Expected an option word in condition phrase.");
                return new OptionConditionPhrase
                {
                    IsNegated = isNegated,
                    OptionWord = optionWord,
                    Arguments = arguments
                };
            }
        }

        /// <summary>
        /// Parse an identifier: a sequence of letters, digits, or underscores starting with a letter.
        /// </summary>
        private string ParseIdentifier()
        {
            SkipWhitespace();
            StringBuilder sb = new StringBuilder();
            if (IsEnd() || !char.IsLetter(Peek()))
                throw new Exception("Expected identifier at position " + _position);
            while (!IsEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
            {
                sb.Append(Consume());
            }
            return sb.ToString().Trim();
        }

        // --- Helpers ---
        private void SkipWhitespace()
        {
            while (!IsEnd() && char.IsWhiteSpace(Peek()))
                Consume();
        }

        private bool IsEnd() => _position >= _input.Length;

        private char Peek() => _input[_position];

        private char Consume() => _input[_position++];

        private bool Match(string s)
        {
            if (_position + s.Length > _input.Length)
                return false;
            return _input.Substring(_position, s.Length) == s;
        }
    }
}
