using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace GravyLang.IteratorLexer
{
    class IteratorLexer
    {
        Itr itr = new Itr();
        StringBuilder currentItem = new StringBuilder();

        //note to self: Create separate logic for checking double + delimiters that are symbols, + separate array
        readonly Delimiter[] __Updated_Delimiters__ =
        {
            new Delimiter() { toMatch = "(" },
            new Delimiter() { toMatch = ")" },
            new Delimiter() { toMatch = "=" },
            new Delimiter() { toMatch = "+" },
            new Delimiter() { toMatch = "/" },
            new Delimiter() { toMatch = "-" },
            new Delimiter() { toMatch = "*" },
            new Delimiter() { toMatch = "!" },
            new Delimiter() { toMatch = "?" },
            new Delimiter() { toMatch = "[" },
            new Delimiter() { toMatch = "]" },
            new Delimiter() { toMatch = ":" },
            new Delimiter() { toMatch = ";" },
            new Delimiter()
            {
                toMatch = ".",
                RegexPreMatch = @"[^\d]",
                RegexPostMatch =  @"[^\d]"
            },
        };

        readonly Delimiter[] __multi_symbol_delimiters__ =
        {
            new Delimiter() { toMatch = "!=" },
            new Delimiter() { toMatch = "==" },
            new Delimiter() { toMatch = "+=" },
            new Delimiter() { toMatch = "-=" },
            new Delimiter() { toMatch = "/=" },
            new Delimiter() { toMatch = "*=" },
            new Delimiter() { toMatch = "++" },
            new Delimiter() { toMatch = "--" },
        };

        public IEnumerable GenerateLexemes(string input)
        {
            yield return GetWhiteSpace(input);
            itr.Line = TrimWhiteSpace(input, itr);

            while (itr.MoveNext())
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

            if (itr.Mode == LexerMode.String)
                yield return "+";
            yield return @"\n";
        }

        private IEnumerable<string> NormalMode(Itr itr)
        {
            if (!SwitchMode(itr.Mode))
            {
                if (' '.Equals(itr.Current()))
                {
                    if (currentItem.Length > 0)
                        yield return PopString(currentItem);
                }
                else if (MatchMultiSymbolDelimiter(__multi_symbol_delimiters__, itr))
                {
                    if (currentItem.Length > 0)
                        yield return PopString(currentItem);
                    currentItem.Append(itr.Current());
                    currentItem.Append(itr.Peek(1));
                    itr.MoveNext();
                }
                else if (MatchDelimiter(__Updated_Delimiters__, itr))
                {
                    if (currentItem.Length > 0)
                        yield return PopString(currentItem);
                    yield return itr.Current().ToString();
                }
                else
                    currentItem.Append(itr.Current());
                if (itr.IsFinalIndex() && currentItem.Length > 0)
                    yield return PopString(currentItem);
            }
            else if (itr.Current() != ' ' && currentItem.Length > 0)
                yield return PopString(currentItem);

        }

        private IEnumerable StringMode(Itr itr)
        {
            if (currentItem.Length == 0)
                currentItem.Append('"');
            currentItem.Append(itr.Current());
            if (SwitchMode(itr.Mode))
                yield return PopString(currentItem);
            else
            {
                if (itr.IsFinalIndex())
                {
                    currentItem.Append('"');
                    yield return PopString(currentItem);
                }
                else if('\\'.Equals(itr.Current()))
                {
                    currentItem.Append(itr.Peek(1));
                    itr.MoveNext(); //don't check next character since already added
                }
            }
                
        }

        private void CommentMode(Itr itr)
        {
            SwitchMode(itr.Mode);
        }

        /// <summary>
        /// Switches lexing mode if necessary, returns true if switch was performed
        /// </summary>
        private bool SwitchMode(LexerMode mode)
        {
            if (mode == LexerMode.Normal)
            {
                if ('"'.Equals(itr.Current()))
                {
                    itr.Mode = LexerMode.String;
                    return true;
                }
                if ('/'.Equals(itr.Current()))
                {
                    if ('/'.Equals(itr.Peek(1)))
                    {
                        itr.Mode = LexerMode.Comment;
                        return true;
                    }
                    if ('*'.Equals(itr.Peek(1)))
                    {
                        itr.Mode = LexerMode.MultiComment;
                        return true;
                    }
                }
            }
            else if (mode == LexerMode.String)
            {
                if(itr.Current() == '"')
                {
                    itr.Mode = LexerMode.Normal;
                    return true;
                }
            }
            else
            {
                if (mode == LexerMode.Comment)
                {
                    if (itr.IsFinalIndex())
                    {
                        itr.Mode = LexerMode.Normal;
                        return true;
                    }
                }
                else if (mode == LexerMode.MultiComment)
                {
                    if (itr == "*/")
                    {
                        itr.Mode = LexerMode.Normal;
                        itr.MoveNext(); //move to '/' 
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool MatchDelimiter(Delimiter[] delimiters, Itr itr)
        {
            foreach (Delimiter del in delimiters)
            {
                if (itr != del.toMatch)
                    continue;

                if (del.RegexPostMatch != null)
                {
                    if (itr.IsFinalIndex())
                        return false;
                    if (!Regex.IsMatch(itr.Peek(1).ToString(), del.RegexPostMatch))
                        continue;
                }
                else if (del.PostMatch != null)
                {
                    var match = false;
                    foreach (string str in del.PostMatch)
                    {
                        if (itr.PeekAndEquals(del.toMatch.Length, str))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                        continue;
                }

                if (del.RegexPreMatch != null)
                {
                    if (itr.IsFirstIndex())
                        return false;
                    if (!Regex.IsMatch(itr.Peek(-1).ToString(), del.RegexPreMatch))
                        continue;
                }
                else if (del.PreMatch != null)
                {
                    var match = false;
                    foreach (string str in del.PreMatch)
                    {
                        if (itr.PeekAndEquals(-str.Length, str))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                        continue;
                }

                return true;
            }
            return false;
        }

        private static bool MatchMultiSymbolDelimiter(Delimiter[] delimiters, Itr itr)
        {
            foreach (Delimiter del in delimiters)
            {
                if(itr == del.toMatch)
                {
                    return true;
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

        private static string GetWhiteSpace(string input)
        {
            if (input == null)
                throw new NullReferenceException("input cannot be null");
            if (string.IsNullOrEmpty(input))
                return "0";
            int indentation = 0;
            for(int i = 0; input[i] == ' '; ++i)
                indentation++;
            return indentation.ToString();
        }

        private static string TrimWhiteSpace(string input, Itr itr)
        {
            if (input == null)
                throw new NullReferenceException("input cannot be null");
            return input.TrimStart();
        }
    }
}
