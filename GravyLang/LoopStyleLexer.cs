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

    public class LoopStyleLexer
    {

        PreviousDelimiter previousDelimiter = new PreviousDelimiter() { del = "Delimiter_Not_Set", index = 0 };
        LexerMode lexerMode = LexerMode.Normal;

        public IEnumerable Lex(string input) {
            
            char[] expressionDelimiters = { '(', ')', '=', '+', '/', '-', '!', '?', '.', '[', ']', ':', ';'};
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

            input = input.Trim(' ');

            for (int i = 0; i < input.Length; ++i) {
                bool finalIndex = (i + 1 == input.Length);
                switch (lexerMode)
                {
                    case LexerMode.String:
                        {
                            if (input[i] == '"' && i != previousDelimiter.index)
                            {
                                string substring = i - 1 == previousDelimiter.index ?
                                    "\"\"" :
                                    input.Substring(previousDelimiter.index, i + 1 - previousDelimiter.index);

                                var previousDel = previousDelimiter;
                                previousDelimiter.index = i;
                                lexerMode = LexerMode.Normal;

                                StringBuilder str = new StringBuilder();
                                str.Append(substring);

                                if (previousDel.del.Equals("Multi_Line_String"))
                                    str.Insert(0,'"');

                                yield return str.ToString();
                            } 
                            else if (finalIndex)
                            {
                                string substring = input.Substring(previousDelimiter.index, i + 1 - previousDelimiter.index);
                                StringBuilder str = new StringBuilder();
                            
                                str.Append(substring);
                                if (previousDelimiter.del.Equals("Multi_Line_String"))
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
                            if (input[i] == '"')
                            {
                                previousDelimiter = new PreviousDelimiter() { del = "\"", index = i };
                                lexerMode = LexerMode.String;
                                if (finalIndex)
                                    goto case LexerMode.String;
                                
                                break;
                            }
                            //else if comment (need to implement full string checking method first)
                            else if (input[i] == ' ')
                            {
                                string substring = previousDelimiter.del == "Delimiter_Not_Set" ?
                                        input.Substring(0, i) :
                                        input.Substring(previousDelimiter.index + 1, i - previousDelimiter.index - 1);

                                previousDelimiter.index = i;
                                previousDelimiter.del = " ";

                                if (substring == "\n" || !string.IsNullOrWhiteSpace(substring))
                                    yield return substring;
                                break;
                            }
                            else
                            {
                                bool exit = false;
                                foreach (string del in allDelimiters)
                                {
                                    if (input[i].ToString().Equals(del))
                                    {
                                        var previousDel = previousDelimiter;
                                        previousDelimiter.index = i;
                                        previousDelimiter.del = del;

                                        if (previousDel.index == i - 1)
                                            yield return del;
                                        else if (previousDel.del == "Delimiter_Not_Set")
                                            yield return input.Substring(0, i + 1);
                                        else
                                        {
                                            yield return input.Substring(previousDel.index + 1, i - previousDel.index - 1);
                                            yield return del;
                                        }
                                        exit = true;
                                        break ;
                                    }
                                }
                                if (exit)
                                    break;
                            }

                            if (finalIndex)
                                yield return previousDelimiter.index == 0 ? input : input.Substring(previousDelimiter.index + 1);
                            break;
                        }
                }

                if (finalIndex)
                    previousDelimiter.index = 0;
            }
            yield return "\n";
        }
    }
}
