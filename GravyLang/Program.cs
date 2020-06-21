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
                if (str != "\n")
                    Console.WriteLine("[" + str + "]");
                else
                    Console.WriteLine("[" + "\\n" + "]");
            }

            foreach (string str in lexer.Lex("comment */ after comment with \"quote something //with comment /*comment*/\""))
            {
                if (str != "\n")
                    Console.WriteLine("[" + str + "]");
                else
                    Console.WriteLine("[" + "\\n" + "]");
            }

            Console.WriteLine("-------------End of Normal Lexer--------------");

            LoopStyleLexer lexer2 = new LoopStyleLexer();

            foreach(string str in lexer2.Lex("\"testing\" that strings \"work properly with"))
            {
                Console.WriteLine("[" + str + "]");
            }

            foreach (string str in lexer2.Lex("multi line"))
            {
                Console.WriteLine("[" + str + "]");
            }

            foreach (string str in lexer2.Lex("strings and stuff\""))
            {
                Console.WriteLine("[" + str + "]");
            }

        }
    }
}
