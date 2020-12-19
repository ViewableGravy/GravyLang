using GravyLang.IteratorLexer;
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

            Console.WriteLine("-------------Start of lexer 2--------------");

            LoopStyleLexer lexer2 = new LoopStyleLexer();

            foreach(string str in lexer2.Lex(")))))"))
            {
                Console.WriteLine("[" + (str == "\n" ? "\\n" : str) + "]");
            }

            Console.WriteLine("-------------Start of lexer 3--------------");
            IteratorLexer.IteratorLexer lexer3 = new IteratorLexer.IteratorLexer();
            foreach (string str in lexer3.Lex("and?????this(us)/*something*/after/*"))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.Lex("some other?thing*/onlythingonthisline///* //something */"))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.Lex("if some _if_ if ( if(if( if("))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.Lex("  for for( (for) foreach( foreach("))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.Lex("    \"this is a string\" and+\"this is"))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.Lex("         a multi line string\""))
            {
                Console.Write("[" + @str + "]");
            }
            Console.WriteLine();

        }
    }
}
