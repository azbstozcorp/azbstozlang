using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lexer {
    public class LexerRule<TokenType> where TokenType : struct, IConvertible {
        public readonly TokenType Type;
        public readonly Regex Regex;
        public bool Enabled { get; set; } = true;

        public LexerRule(TokenType name, string pattern) {
            if (!typeof(TokenType).IsEnum) throw new ArgumentException("inName must be an enum.");

            Type = name;
            Regex = new Regex(pattern, RegexOptions.Multiline);
        }

        public bool Toggle() {
            Enabled ^= true;
            return Enabled;
        }

        public static implicit operator LexerRule<TokenType>((TokenType type, string pattern) pair) => new LexerRule<TokenType>(pair.type, pair.pattern);
    }

    public class LexerToken<TokenType> where TokenType : struct, IConvertible {
        public readonly bool IsNullMatch = false;
        public readonly LexerRule<TokenType> Rule = null;
        public readonly Match RegexMatch;

        public TokenType Type { get { try { return Rule.Type; } catch (NullReferenceException) { return default(TokenType); } } }

        private string nullValueData;
        public string Value => IsNullMatch ? nullValueData : RegexMatch.Value;

        public LexerToken(LexerRule<TokenType> rule, Match match) {
            Rule = rule;
            RegexMatch = match;
        }
        public LexerToken(string unknown) {
            IsNullMatch = true;
            nullValueData = unknown;
        }

        public override string ToString() {
            return $"{Type} [{Value}]";
        }
    }

    public class LexerBase<TokenType> where TokenType : struct, IConvertible {
        public List<LexerRule<TokenType>> Rules { get; private set; } = new List<LexerRule<TokenType>>();

        public int CurrentLineNumber { get; private set; } = 0;
        public int CurrentLinePos { get; private set; } = 0;
        public int TotalCharsScanned { get; private set; } = 0;
        public bool Verbose { get; set; } = true;

        StreamReader textStream;

        public LexerBase() { }

        public void AddRule(LexerRule<TokenType> rule) => Rules.Add(rule);
        public void AddRules(IEnumerable<LexerRule<TokenType>> rules) => Rules.AddRange(rules);

        public void Initialize(StreamReader reader) { textStream = reader; }

        public IEnumerable<LexerToken<TokenType>> TokenStream() {
            string nextLine;
            List<LexerToken<TokenType>> matches = new List<LexerToken<TokenType>>();
            while ((nextLine = textStream.ReadLine()) != null) {
                CurrentLinePos = 0;

                while (CurrentLinePos < nextLine.Length) {
                    matches.Clear();
                    foreach (LexerRule<TokenType> rule in Rules) {
                        if (!rule.Enabled) continue;

                        Match nextMatch = rule.Regex.Match(nextLine, CurrentLinePos);
                        if (!nextMatch.Success) continue;

                        matches.Add(new LexerToken<TokenType>(rule, nextMatch));
                    }

                    if (matches.Count == 0) {
                        string unknownTokenContent = nextLine.Substring(CurrentLinePos);
                        if (Verbose) Console.WriteLine("[Unknown Token: No matches found for this line] {0}", unknownTokenContent);
                        yield return new LexerToken<TokenType>(unknownTokenContent);
                        break;
                    }

                    matches.Sort((LexerToken<TokenType> a, LexerToken<TokenType> b) => {
                        // Match of offset position position
                        int result = nextLine.IndexOf(a.RegexMatch.Value, CurrentLinePos, StringComparison.CurrentCulture) -
                                    nextLine.IndexOf(b.RegexMatch.Value, CurrentLinePos, StringComparison.CurrentCulture);
                        // If they both start at the same position, then go with the longest one
                        if (result == 0)
                            result = b.RegexMatch.Length - a.RegexMatch.Length;

                        return result;
                    });
                    LexerToken<TokenType> selectedToken = matches[0];
                    int selectedTokenOffset = nextLine.IndexOf(selectedToken.RegexMatch.Value, CurrentLinePos) - CurrentLinePos;

                    if (selectedTokenOffset > 0) {
                        string extraTokenContent = nextLine.Substring(CurrentLinePos, selectedTokenOffset);
                        CurrentLinePos += selectedTokenOffset;
                        if (Verbose) Console.WriteLine("[Unmatched content] '{0}'", extraTokenContent);
                        yield return new LexerToken<TokenType>(extraTokenContent);
                    }

                    CurrentLinePos += selectedToken.RegexMatch.Length;
                    if (Verbose) Console.WriteLine(selectedToken);
                    yield return selectedToken;
                }

                if (Verbose) Console.WriteLine("[Lexer] Next line");
                CurrentLineNumber++;
                TotalCharsScanned += CurrentLinePos;
            }
        }

        public void EnableRule(TokenType type) => SetRule(type, true);
        public void DisableRule(TokenType type) => SetRule(type, false);
        public void SetRule(TokenType type, bool state) {
            foreach (LexerRule<TokenType> rule in Rules)
                if (Enum.GetName(rule.Type.GetType(), rule.Type) == Enum.GetName(type.GetType(), type)) {
                    rule.Enabled = state;
                    return;
                }

        }

        public void EnableRulesByPrefix(string tokenTypePrefix) => SetRulesByPrefix(tokenTypePrefix, true);
        public void DisableRulesByPrefix(string tokenTypePrefix) => SetRulesByPrefix(tokenTypePrefix, false);
        public void SetRulesByPrefix(string tokenTypePrefix, bool state) {
            foreach (LexerRule<TokenType> rule in Rules) {
                if (Enum.GetName(rule.Type.GetType(), rule.Type).StartsWith(tokenTypePrefix, StringComparison.CurrentCulture)) {
                    rule.Enabled = state;
                }
            }
        }
    }
}
