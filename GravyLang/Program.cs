using System;
using System.Collections.Generic;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            var temp = new Dictionary<string, string[]>()
            {
                {"\"multi line", new[] { "0", "\"multi line\"", "\n" } },
                {"string\"", new[] { "0", "\"string\"", "\n"} }
            };

            foreach (string str in lexer.Lex("int i=Function(\"random word\").test +1; elseif if;()if a( something) elseelse selse( else \" unfinished string "))
            {
                if (str != "\n")
                    Console.WriteLine("[" + str + "]");
                else
                    Console.WriteLine("[" + "\\n" + "]");
            }

            foreach (string str in lexer.Lex("rest of string\" wow this works\" test \"?!"))
            {
                if (str != "\n")
                    Console.WriteLine("[" + str + "]");
                else
                    Console.WriteLine("[" + "\\n" + "]");
            }

        }
    }
}
