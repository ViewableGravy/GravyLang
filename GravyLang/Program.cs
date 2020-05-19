using System;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            foreach(string str in lexer.Lex("int i=Function(\"random word\").test +1"))
            {
                Console.WriteLine(str);
            }
            
        }
    }
}
