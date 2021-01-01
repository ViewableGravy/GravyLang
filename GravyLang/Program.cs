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

            foreach(Token token in lexer.LexFile("LexerTester.txt"))
            {
                if(token.Value == @"\n")
                    Console.WriteLine("[" + token.Classification + ", " + token.Value + "]");
                else
                    Console.Write("[" + token.Classification + ", " + token.Value + "]");
            }

        }
    }
}
