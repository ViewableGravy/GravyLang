using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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
        public string[] PreMatch;
        public string toMatch;
        public string[] PostMatch;
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

        public char PeekForward(int i)
        {
            if (index + i < line.Length)
                return line[index + i];
            throw new IndexOutOfRangeException();
        }

        public char PeekBackward(int i)
        {
            if (index - i > -1)
                return line[index - i];
            throw new IndexOutOfRangeException();
        }

        public static bool operator ==(Itr itr, string str)
        {
            if (str[0] != itr.Current())
                return false;
            if (itr.index + str.Length > itr.line.Length)
                return false;
            for (int i = 1; i < str.Length; ++i)
                if (!str[i].Equals(itr.PeekForward(i)))
                    return false;
            return true;
        }

        public static bool operator !=(Itr itr, string str)
        {
            return !(itr == str);
        }
    }

    public class LoopStyleLexer
    {

        PreviousDelimiter previousDelimiter = new PreviousDelimiter() { del = "Delimiter_Not_Set", index = 0 };
        LexerMode lexerMode = LexerMode.Normal;

        Itr itr = new Itr();
        StringBuilder currentItem = new StringBuilder();

        Delimiter[] __Updated_Delimiters__ = {
                //new Delimiter() { toMatch = " " }, special case
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
                    PostMatch = new[] { @"[^\S\d\w]" }
                }
                //new Delimiter() {
                //    toMatch = "else",
                //    PreMatch = new[] { " " },
                //    PostMatch = new[] { " " }
                //}
            };

        public IEnumerable Lex2(string input)
        {
            itr.Line = TrimWhiteSpace(input, itr);

            while(itr.MoveNext())
            { 
                switch (itr.Mode)
                {
                    case LexerMode.Normal: 
                        foreach (var result in NormalMode(itr))
                            yield return result;
                        break;
                    case LexerMode.String:
                        foreach (var result in StringMode(itr))
                            yield return result;
                        break;
                    case LexerMode.Comment:
                    case LexerMode.MultiComment:
                        CommentMode(itr);
                        break;
                }
            }

            itr.Reset();
        }

        private bool MatchDelimiter(Delimiter[] delimiters, Itr itr)
        {
            foreach (Delimiter del in delimiters)
                if (del.toMatch.Equals(itr.Current().ToString()))
                    return true;
            return false;
        }

        private IEnumerable<string> NormalMode(Itr itr)
        {
            if (!SwitchMode(itr.Mode))
            {
                //check if white space (to discard)
                if(' '.Equals(itr.Current()))
                {
                    if(currentItem.Length > 0)
                    {
                        yield return currentItem.ToString();
                        currentItem.Clear();
                    }
                } 
                else
                {
                    if (MatchDelimiter(__Updated_Delimiters__, itr))
                    {
                        if (currentItem.Length > 0)
                        {
                            yield return currentItem.ToString();
                            currentItem.Clear();
                        }
                        yield return itr.Current().ToString();
                    }
                    else
                    {
                        currentItem.Append(itr.Current());
                    }
                } 
            }
            else
            {
                if(itr.Current() != ' ' && currentItem.Length > 0)
                    yield return PopString(currentItem);
                if (itr.Mode == LexerMode.String)
                    currentItem.Append(itr.Current());
            }
        }

        private IEnumerable StringMode(Itr itr)
        {
            throw new NotImplementedException();
        }

        private void CommentMode(Itr itr)
        {
            SwitchMode(itr.Mode);
        }

        /// <summary>
        /// Switches lexing mode if necessary, returns true if switch was performed
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private bool SwitchMode(LexerMode mode)
        {
            if (mode == LexerMode.Normal)
            {
                if ('"'.Equals(itr.Current()))
                {
                    itr.Mode = LexerMode.String;
                    return true;
                }
                if ('/'.Equals(itr.Current())) {
                    if ('/'.Equals(itr.PeekForward(1))) {
                        itr.Mode = LexerMode.Comment;
                        return true;
                    }
                    if ('*'.Equals(itr.PeekForward(1)))
                    {
                        itr.Mode = LexerMode.MultiComment;
                        return true;
                    }
                }
            }
            else if (mode == LexerMode.String)
            {
                throw new NotImplementedException("Have not implemented string handling");
            }
            else
            {
                if (mode == LexerMode.Comment)
                {
                    if(itr.Index == -1)
                    {
                        itr.Mode = LexerMode.Normal;
                        return true;
                    }
                }
                else if (mode == LexerMode.MultiComment)
                {
                    if(itr == "*/")
                    {
                        itr.Mode = LexerMode.Normal;
                        itr.MoveNext(); //move to '/' 
                        return true;
                    }
                }
            }
            
            return false;
        }

        private static string PopString(StringBuilder builder)
        {
            if (builder.Length == 0)
                throw new NullReferenceException();

            string result = builder.ToString();
            builder.Clear();
            return result;
        }

        private static string TrimWhiteSpace(string input, Itr itr)
        {
            if (input == null)
                throw new NullReferenceException("input cannot be null");
            if (itr.Mode != LexerMode.String)
                return input.TrimStart();
            return input;
        }

        private bool MatchDelimiter(Itr itr)
        {
            //itr.current == [0] of any delimiters
            //from those: itr.current == [1], repeat
            //delimiter.length <? check pre del
            //check post del
            throw new NotImplementedException();
        }

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
