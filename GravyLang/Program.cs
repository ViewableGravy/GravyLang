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

        }
    }
}
