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

        Negative,
        Positive,

        OParen,
        CParen,
        OSquare,
        CSquare,
        OBrace,
        CBrace,

        Integer,
    }

    public class FyreLangLexer : LexerBase<FyreLangToken> {
        public FyreLangLexer() {
            AddRules(
                new List<LexerRule<FyreLangToken>>() {
                    (Negative, @"-"),
                    (Positive, @"+"),

                    (OParen, @"\("),
                    (CParen, @"\)"),
                    (OSquare, @"\["),
                    (CSquare, @"\]"),
                    (OBrace, @"\{"),
                    (CBrace, @"\}"),

                    (Integer, @"\b\d+\b"),
                });
        }
    }
}
