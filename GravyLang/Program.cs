using System;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            lexer.Lex("int i=Function(\"random word\").test +1");
            
        }
    }
}
