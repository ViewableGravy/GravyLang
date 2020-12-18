using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GravyLang
{
    struct PreviousDelimiter
    {
        public string del;
        public int index;
    }

    public enum LexerMode
    {
        Normal,
        Comment,
        MultiComment,
        String
    }

    struct StringAfterIndexResult
    {
        public bool found;
        public PreviousDelimiter? previousDelimiterInformation;
    }

    struct Delimiter
    {
        private string regexPreMatch;
        private string[] preMatch;
        public string toMatch;
        private string regexPostMatch;
        private string[] postMatch;

        public string RegexPreMatch 
        {
            get { return regexPreMatch; }
            set
            {
                if (PreMatch != null)
                    throw new InvalidOperationException();
                regexPreMatch = value;
            }
        }

        public string[] PreMatch
        {
            get { return preMatch; }
            set
            {
                if (regexPreMatch != null)
                    throw new InvalidOperationException();
                preMatch = value;
            }
        }

        public string RegexPostMatch
        {
            get { return regexPostMatch; }
            set
            {
                if (postMatch != null)
                    throw new InvalidOperationException();
                regexPostMatch = value;
            }
        }

        public string[] PostMatch
        {
            get { return postMatch; }
            set
            {
                if (regexPostMatch != null)
                    throw new InvalidOperationException();
                postMatch = value;
            }
        }

    }

    public class Itr : IEnumerator, IEnumerable
    {
        private string line;
        private LexerMode mode;
        private int index = -1;
        private bool multiLineString = false;
        private bool multiLineComment = false;

        public LexerMode Mode 
        { 
            get => mode;
            set
            {
                if (value == LexerMode.Comment && this.mode == LexerMode.String)
                    throw new InvalidOperationException();
                mode = value;
            }
        }
        public int Index { get => index; }
        public string Line 
        { 
            set 
            {
                if (line != null && Index != line.Length && index != -1)
                    throw new InvalidOperationException("Line was not yet finished processing");
                line = value;
            }
        }

        public bool MoveNext()
        {
            index++;
            return (index < line.Length);
        }

        public void Reset()
        {
            index = -1;
        }

        object IEnumerator.Current
        {
            get {
                return line[index];
            }
        }

        public bool MultiLineString 
        { 
            get => multiLineString; 
            set
            {
                if (multiLineComment)
                    throw new InvalidOperationException("Cannot have multi-line string/comment at the same time");
                multiLineString = value;
            }
        }

        public bool MultiLineComment 
        { 
            get => multiLineComment;
            set
            {
                if (multiLineString)
                    throw new InvalidOperationException("Cannot have multi-line string/comment at the same time");
                multiLineComment = value;
            }
        }

        public char Current()
        {
            return line[index];
        }

        public IEnumerator GetEnumerator()
        {
            foreach (char c in line)
            {
                yield return c;
            }
        }

        public char Peek(int i)
        {
            if (index + i < line.Length && index + i >= 0)
                return line[index + i];
            throw new IndexOutOfRangeException();
        }

        public bool PeekAndEquals(int i, string str)
        {
            if (index + i + str.Length > this.line.Length || index + i < 0)
                return false;
            if (str[0] != this.Peek(i))
                return false;
            for (int j = 1; j < str.Length; ++j)
                if (!str[j].Equals(this.Peek(i + j)))
                    return false;
            return true;
        }

        public static bool operator ==(Itr itr, string str)
        {
            return itr.PeekAndEquals(0, str);
            /*
            if (str[0] != itr.Current())
                return false;
            if (itr.index + str.Length > itr.line.Length)
                return false;
            for (int i = 1; i < str.Length; ++i)
                if (!str[i].Equals(itr.PeekForward(i)))
                    return false;
            return true;
            */
        }

        public static bool operator !=(Itr itr, string str)
        {
            return !(itr == str);
        }

        public bool IsFinalIndex()
        {
            return index == line.Length - 1;
        }

        public bool IsFirstIndex()
        {
            return index == 0;
        }
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
                            //start of string
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
