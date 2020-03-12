using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer {
    public enum FyreLangToken {

    }

    public class FyreLangLexer : LexerBase<FyreLangToken> {
        public FyreLangLexer() {
            AddRules(
                new List<LexerRule<FyreLangToken>>() {

                });
        }
    }
}
