using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace GravyLang
{
    public class Lexer
    {

        private bool stillString = false;
        private bool stillComment = false;
        //takes a line and returns it as a string[]
        public string[] Lex(string languageInput)
        {

            char[] expressionDelimiters = {'(', ')', '=', '+', '/', '-', '!', '?', '.', '[', ']', ':', ';', ' ' };
            string[] statementDelimiters = { "if", "else"};
            string[] typeDelimiters = { "int", "string" };

            string[] allDelimiters = Array.Empty<string>()
                .Concat(expressionDelimiters.Select(x => x.ToString()))
                .Concat(statementDelimiters)
                .Concat(typeDelimiters)
                .ToArray();

            //determine number of spaces of indentation
            string[] indentation = string.IsNullOrWhiteSpace(languageInput) ?
                    new[] { "0" } :
                    new[] { languageInput.TakeWhile(Char.IsWhiteSpace).Count().ToString() };

            //handle comments -- to be implemented
            string handleComment = HandleComments(languageInput);

            //handle strings
            string[] handleStrings = handleComment.Length > 0 ?
                indentation.Concat(HandleStringMultiLine(handleComment).Where(s => !string.IsNullOrWhiteSpace(s))).ToArray() :
                indentation;

            //create tokens
            string[] tokens = handleStrings.Length > 1 ? indentation : handleStrings;
            for(int i = 1; i < handleStrings.Length; ++i)
                tokens = handleStrings[i][0].Equals('"') ?
                    tokens.Concat(new string[] { handleStrings[i] }).ToArray() :
                    tokens.Concat(InclusiveSplit(handleStrings[i], allDelimiters.ToList())).ToArray();

            //add the new line
            string[] finalTokens = tokens.Concat(new string[] { "\n" }).ToArray();
            //Generate token key value pairs
            Dictionary<string, string> tokenKeyValuePairs = GenerateTokenKeys(tokens);
            //return Lexical Analysis
            return finalTokens;
        }

        /// <summary>
        /// to implement : takes the array of tokens and associates them with an identifier
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private Dictionary<string,string> GenerateTokenKeys(string[] tokens)
        {
            //handle opening

            //handle middle

            //handle ending

            //return TokenKeys
            return null;
        }

        private string HandleComments(string input)
        {   
            int firstIndex = input.IndexOf("//");
            if (firstIndex == -1)
                return input ;
            if (!IsInString(input, firstIndex))
                return firstIndex == 0 ? String.Empty : input.Substring(0, firstIndex - 1);
            return input.Substring(0, firstIndex + 2) + HandleComments(input.Substring(firstIndex + 2));
        }

        private string[] HandleStringMultiLine(string input)
        {
            int firstIndex = input.IndexOf('"');
            if (firstIndex == -1)
                return stillString ? new[] { $"\"{input}\"", "+" } : new[] { input };
            //else

            if (stillString)
            {
                stillString = false;
                return new[] { $"\"{ input.Substring(0, firstIndex) }\"" }
                    .Concat(HandleStringMultiLine(input.Substring(firstIndex + 1)))
                    .ToArray();
            }
            else
            {
                int secondIndex = firstIndex;

                if (firstIndex + 1 < input.Length)
                    if ((secondIndex = input.IndexOf("\"", firstIndex + 1)) == -1)
                    {
                        stillString = true;
                        return new[] {
                            input.Substring(0, firstIndex),
                            $"\"{input.Substring(firstIndex + 1)}\"",
                            "+"
                        };
                    }

                List<string> output = new List<string>();
                if (firstIndex > 0)
                    output.Add(input.Substring(0, firstIndex - 1));
                output.Add(input.Substring(firstIndex, secondIndex - firstIndex + 1));
                if (secondIndex + 1 < input.Length)
                    output = output.Concat(HandleStringMultiLine(input.Substring(secondIndex + 1))).ToList();
                if (firstIndex + 1 == input.Length)
                {
                    stillString = true;
                    output.Add("+");
                }

                return output.ToArray();
            }
        }

        private string[] InclusiveSplit(string input, List<string> delimiters)
        {
            List<string> result = InclusiveSplit(input, delimiters.First()).ToList();
            delimiters.Remove(delimiters.First());

            if (delimiters.Count == 0)
                return result.ToArray();

            List<string> finalResults = new List<string>();

            if (result.Any())
                for (int i = 0; i < result.Count; ++i)
                {
                    string[] temp = InclusiveSplit(result[i], new List<string>(delimiters));
                    if (temp.Any())
                        finalResults.AddRange(temp);
                }

            return finalResults.ToArray();
        }

        private string[] InclusiveSplit(string input, string delimiter)
        {
            int previousIndex = 0;
            while (previousIndex < input.Length)
            {
                int index = input.IndexOf(delimiter, previousIndex);
                if (index == -1)
                    break;

                List<string> output = new List<string>();
                if (ProcessDelimiter(input, delimiter, index))
                {
                    if (index != 0)
                        output.Add(input.Substring(0, index));
                    output.Add(delimiter);
                    output = output.Concat(InclusiveSplit(input.Substring(index + delimiter.Length), delimiter)).ToList();
                    return output.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                }
                previousIndex = index + delimiter.Length;
            }
            return new string[] { input };
                    
        }

        private static bool ProcessDelimiter(string input, string delimiter, int index)
        {
            //performs a check on the input based on the delimiter (e.g. if must have bracket after it and a space before it)
            switch (delimiter)
            {
               case "if":
                    {
                        //if previous character isn't else or ' ';
                        if (index != 0)
                            if (EndOfStringIs(input.Substring(0,index - 1), "else") || input[index - 1] != ' ')
                                return false;
                            
                        int i = index + delimiter.Length;
                        while (i < input.Length)
                        {
                            if (input[i] == '(')
                                return true;
                            if (input[i++] != ' ')
                                return false;
                        }
                        return false;
                    }
                case "else":
                    {
                        if (index != 0)
                            if (input[index - 1] != ' ')
                                return false;
                                
                        int i = index + delimiter.Length;
                        if (i < input.Length)
                            return !StartOfStringIs(input.Substring(i), "if") &&
                                StartOfStringIs(input.Substring(i), " ");

                        return false;
                    }
                default:
                    return true;
            }
        }

        private static bool StartOfStringIs(string input, string toCheckFor)
        {
            for(int i = 0; i < toCheckFor.Length; ++i)
                if (!input[i].Equals(toCheckFor[i]))
                    return false;
            return true;
        }

        private static bool EndOfStringIs(string input, string toCheckFor)
        {
            for (int i = input.Length - 1; i > input.Length - 1 - toCheckFor.Length; --i)
                if (!input[i].Equals(toCheckFor[i - (input.Length - toCheckFor.Length)]))
                    return false;
            return true;
        }

        private bool IsInString(string input, int index)
        {
            return input.Substring(0, index).Count(c => c == '"') % 2 == (stillString ? 0 : 1);
        }
    }
}
