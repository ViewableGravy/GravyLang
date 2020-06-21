using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GravyLang
{
    struct PreviousDelimiter
    {
        public string del;
        public int index;
    }

    enum LexerMode
    {
        Normal,
        Comment,
        String
    }

    class LoopStyleLexer
    {

        PreviousDelimiter previousDelimiter = new PreviousDelimiter() { del = "", index = 0 };
        LexerMode lexerMode = LexerMode.Normal;

        public IEnumerable Lex(string input) {
            
            char[] expressionDelimiters = { '(', ')', '=', '+', '/', '-', '!', '?', '.', '[', ']', ':', ';', ' '};
            string[] statementDelimiters = { "if", "else" };
            string[] typeDelimiters = { "int", "string" };

            string[] allDelimiters = Array.Empty<string>()
                .Concat(expressionDelimiters.Select(x => x.ToString()))
                .Concat(statementDelimiters)
                .Concat(typeDelimiters)
                .ToArray();

            yield return string.IsNullOrWhiteSpace(input) ?
                    "0" :
                    input.TakeWhile((c => c != '\n'))
                    .TakeWhile(Char.IsWhiteSpace)
                    .Count().ToString();

            for (int i = 0; i < input.Length; ++i) {
                switch (lexerMode)
                {
                    case LexerMode.String:
                    {
                        if (input[i] == '"')
                        {
                            var previousDel = previousDelimiter;
                            previousDelimiter.index = i;
                            lexerMode = LexerMode.Normal;

                            string substring = input.Substring(previousDel.index, i + 1 - previousDel.index);

                            StringBuilder str = new StringBuilder();
                            str.Append(substring);

                            if (previousDel.del.Equals("Multi_Line_String"))
                                str.Insert(0,'"');

                            yield return str.ToString();

                            continue;
                        }

                        //Is end of line and an unfinished string
                        if (i + 1 == input.Length)
                        {
                            var previousDel = previousDelimiter;

                            string substring = input.Substring(previousDel.index, i + 1 - previousDel.index);
                            StringBuilder str = new StringBuilder();
                            
                            str.Append(substring);
                            if (previousDel.del.Equals("Multi_Line_String"))
                                str.Insert(0, '"');
                            str.Append('"');

                            previousDelimiter.del = "Multi_Line_String";

                            yield return str.ToString();
                            yield return "+";
                        }
                       
                        break;
                    }
                    case LexerMode.Comment:
                    {
                        //to implement
                        break;
                    }
                    case LexerMode.Normal:
                    {
                        if(input[i] == '"')
                        {
                            previousDelimiter = new PreviousDelimiter() { del = "\"", index = i };
                            lexerMode = LexerMode.String;
                            continue;
                        }

                        if (input[i] == ' ')
                        {
                            string substring = input.Substring(previousDelimiter.index + 1, i - previousDelimiter.index - 1);
                            previousDelimiter.index = i;
                            previousDelimiter.del = " ";
                                
                            if (substring == "\n" || !string.IsNullOrWhiteSpace(substring))
                                yield return substring;
                            continue;
                        }

                        foreach (string del in allDelimiters)
                            if (del.Equals(input[i])) 
                            {
                                var previousDel = previousDelimiter;
                                previousDelimiter.index = i;
                                previousDelimiter.del = del;

                                yield return del;
                                yield return input.Substring(previousDel.index + 1, i - previousDel.index);
                            }
                        
                        break;
                    }
                }
                if (i + 1 == input.Length)
                    previousDelimiter.index = 0;
            }
            yield return "\\n";
        }
    }
}
