using System;

namespace GravyLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            foreach(string str in lexer.Lex("int i=Function(\"random word\").test +1; elseif if;()if a( something) elseelse selse( else"))
            {
                if (str != "\n")
                    Console.WriteLine("[" + str + "]");
                else
                    Console.WriteLine("[" + "\\n" + "]");
            }
            
        }
    }
}
