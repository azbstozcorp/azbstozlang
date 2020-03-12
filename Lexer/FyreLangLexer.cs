using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Lexer.FyreLangToken;

namespace Lexer {
    public enum FyreLangToken {
        Unknown = 0,
        EOF,
        Endl,

        Negative,
        Positive,

        OParen,
        CParen,
        OSquare,
        CSquare,
        OBrace,
        CBrace,

        Integer,

        Name,
    }

    public class FyreLangLexer : LexerBase<FyreLangToken> {
        public FyreLangLexer() {
            AddRules(
                new List<LexerRule<FyreLangToken>>() {
                    (Endl, @"(?!.)"),

                    (Negative, @"-"),
                    (Positive, @"\+"),

                    (OParen, @"\("),
                    (CParen, @"\)"),
                    (OSquare, @"\["),
                    (CSquare, @"\]"),
                    (OBrace, @"\{"),
                    (CBrace, @"\}"),

                    (Integer, @"\b\d+\b"),

                    (Name, @"\b[a-z]+\b"),
                });
        }
    }
}
