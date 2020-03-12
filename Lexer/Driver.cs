using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer {
    class Driver {
        static string code = "";

        static void Main(string[] args) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(code);
            writer.Flush();
            stream.Position = 0;
            var reader = new StreamReader(stream);

            FyreLangLexer lexer = new FyreLangLexer();
            lexer.Initialize(reader);
            LexerToken<FyreLangToken>[] tokens = lexer.TokenStream().ToArray();

            writer.Dispose();
            stream.Dispose();
            reader.Dispose();

            foreach (var token in tokens) Console.WriteLine($"{token}");
            Console.ReadLine();
        }
    }
}
