using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Channels;

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

    struct StringAfterIndexResult
    {
        public bool found;
        public PreviousDelimiter? previousDelimiterInformation;
    }

    struct Delimiter
    {
        public string[] PreMatch;
        public string toMatch;
        public string[] PostMatch;
    }

    public class LoopStyleLexer
    {

        PreviousDelimiter previousDelimiter = new PreviousDelimiter() { del = "Delimiter_Not_Set", index = 0 };
        LexerMode lexerMode = LexerMode.Normal;

        public IEnumerable Lex(string input) {

            Delimiter[] __Updated_Delimiters__ = {
                new Delimiter() { toMatch = " " },
                new Delimiter() { toMatch = "(" },
                new Delimiter() { toMatch = ")" },
                new Delimiter() { toMatch = "=" },
                new Delimiter() { toMatch = "+" },
                new Delimiter() { toMatch = "/" },
                new Delimiter() { toMatch = "-" },
                new Delimiter() { toMatch = "*" },
                new Delimiter() { toMatch = "!" },
                new Delimiter() { toMatch = "?" },
                new Delimiter() { toMatch = "." },
                new Delimiter() { toMatch = "[" },
                new Delimiter() { toMatch = "]" },
                new Delimiter() { toMatch = ":" },
                new Delimiter() { toMatch = ";" },
                new Delimiter() { 
                    toMatch = "int", 
                    PreMatch = new[] { " ", "(" }, 
                    PostMatch = new[] { " ", ")" } 
                },
                new Delimiter() {
                    toMatch = "if",
                    PreMatch = new[] { " ", "(", "else" },
                    PostMatch = new[] { " ", "(" }
                }
                //new Delimiter() {
                //    toMatch = "else",
                //    PreMatch = new[] { " " },
                //    PostMatch = new[] { " " }
                //}
            };

            char[] expressionDelimiters = { '(', ')', '=', '+', '/', '-', '*', '!', '?', '.', '[', ']', ':', ';'};
            //string[] statementDelimiters = { "if", "else" };
            //string[] typeDelimiters = { "int", "string" };

            string[] allDelimiters = Array.Empty<string>()
                .Concat(expressionDelimiters.Select(x => x.ToString()))
                //.Concat(statementDelimiters)
                //.Concat(typeDelimiters)
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
                                foreach (Delimiter delimiter in __Updated_Delimiters__)
                                {
                                    StringAfterIndexResult result = Compare(input, i, delimiter.toMatch);

                                    bool Matching = result.found;

                                    //check that preMatch is successful
                                    if (delimiter.PreMatch != null && Matching)
                                    {
                                        Matching = false;

                                        i -= delimiter.PreMatch.Length;
                                        foreach(string PreDelimiter in delimiter.PreMatch)
                                        {
                                            StringAfterIndexResult PreMatchresult = Compare(input, i, PreDelimiter);
                                            if (PreMatchresult.found == true)
                                            {
                                                Matching = true;
                                                break;
                                            }  
                                        }
                                    }

                                    //check that PostMatch is successful
                                    if (delimiter.PostMatch != null && Matching)
                                    {
                                        Matching = false;
                                        i += delimiter.toMatch.Length;
                                        foreach (string PostDelimiter in delimiter.PostMatch)
                                        {
                                            StringAfterIndexResult PostMatchresult = Compare(input, i, PostDelimiter);
                                            if (PostMatchresult.found == true)
                                            {
                                                Matching = true;
                                                break;
                                            }
                                        }
                                    }

                                    if(Matching)
                                    {
                                        //if you made it here then it's a match, Congratulations
                                        var previousDel = previousDelimiter;
                                        previousDelimiter.index = result.previousDelimiterInformation.Value.index;
                                        previousDelimiter.del = result.previousDelimiterInformation.Value.del;

                                        if (previousDel.index == i - 1)
                                            yield return delimiter.toMatch;
                                        else if (previousDel.del == "Delimiter_Not_Set")
                                        {
                                            if (i != 0)
                                                yield return input.Substring(0, i);
                                            yield return delimiter.toMatch;
                                        }
                                        else
                                        {
                                            yield return input.Substring(previousDel.index + 1, i - previousDel.index - 1);
                                            yield return delimiter.toMatch;
                                        }
                                        exit = true;
                                        break;
                                    }
                                }
                                if(exit)
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

        private StringAfterIndexResult Compare(string input, int startIndex, string delimiter)
        {
            int i = startIndex;
            if (input[i] == delimiter.First())
                foreach (char chr in delimiter)
                {
                    if (i < input.Length)
                        if (input[i] == chr && chr == delimiter.Last())
                            return new StringAfterIndexResult
                            {
                                found = true,
                                previousDelimiterInformation =
                                    new PreviousDelimiter() { del = delimiter, index = i }
                            };
                        else
                            break;
                    i++;
                }

            return new StringAfterIndexResult { found = false };

        }
    }
}
