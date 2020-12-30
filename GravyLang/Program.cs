using GravyLang.IteratorLexer;
using System;
using System.Collections.Generic;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-------------Start of multi-line lexer--------------");
            MultiLineLexer lexer = new MultiLineLexer();

            int i = 0;
            foreach(Token token in lexer.LexFile("LexerTester.txt"))
            {
                if(i == 10)
                {
                    i = 0;
                    Console.WriteLine("[" + token.Classification + ", " + token.Value + "]");
                }
                else
                    Console.Write("[" + token.Classification + ", " + token.Value + "]");
                ++i;
            }

        }
    }
}
