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
            Tokenizer tokenizer = new Tokenizer();

            foreach (string str in lexer3.GenerateLexemes("and?????this(us)/*something*/after/*"))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes("some other?thing*/onlythingonthisline///* //something */"))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes("if some _if_ if ( if(if( if("))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes("  for for( (for) foreach( foreach("))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes("    \"this is a string\" and++\"this is"))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes("  //comment /*comment*/       a multi line string\""))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();

            foreach (string str in lexer3.GenerateLexemes(" \" \\n\\r \\n \\r \""))
            {
                Console.Write("[" + tokenizer.GetToken(@str) + ", " + @str + "]");
            }
            Console.WriteLine();
        }
    }
}
