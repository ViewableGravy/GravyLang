using System;
using System.Collections.Generic;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            foreach (string str in lexer.Lex("/* multi line"))
            {
                Console.WriteLine("[" + (str == "\n" ? "\\n" : str) + "]");
            }

            foreach (string str in lexer.Lex("comment */ after comment with \"quote something //with comment /*comment*/\""))
            {
                Console.WriteLine("[" + (str == "\n" ? "\\n" : str) + "]");
            }

            Console.WriteLine("-------------End of Normal Lexer--------------");

            LoopStyleLexer lexer2 = new LoopStyleLexer();

            foreach(string str in lexer2.Lex(")))))"))
            {
                Console.WriteLine("[" + (str == "\n" ? "\\n" : str) + "]");
            }

        }
    }
}
