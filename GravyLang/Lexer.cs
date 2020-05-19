using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GravyLang
{
    class Lexer
    {
        
        //takes a line and returns it as a string[]
        public string[] Lex(string languageInput)
        {
            
            char[] expressionDelimiters = {'(', ')', '=', '+', '/', '-', '.', '[', ']', ':', '\"', ' ' };
            string[] statementDelimiters = { "if"};
            string[] typeDelimiters = { "int", "string" };

            string[] allDelimiters = Array.Empty<string>()
                .Concat(expressionDelimiters.Select(x => x.ToString()))
                .Concat(statementDelimiters)
                .Concat(typeDelimiters)
                .ToArray();

            //Run tests
            Tests(allDelimiters);
            //determine number of spaces of indentation
            string[] indentation = new string[] { languageInput.TakeWhile(Char.IsWhiteSpace).Count().ToString() };
            //split the string into tokens
            string[] tokens = indentation.Concat(InclusiveSplit(languageInput, allDelimiters.ToList())).ToArray();
            //add the new line
            string[] finalTokens = tokens.Concat(new string[] { "/n" }).ToArray();
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

        private void Tests(string[] delimiters)
        {
            Console.WriteLine("------------------ Executing test execution ------------------");
            Console.WriteLine("Test case 1");
            foreach (string str in InclusiveSplit(")))", delimiters.ToList()))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case 2");
            foreach (string str in InclusiveSplit("this is:} a : string that :} is : meaningless :::", delimiters.ToList()))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case 3");
            foreach (string str in InclusiveSplit(") a", ")"))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case 4");
            foreach (string str in InclusiveSplit("if( thisif () else if ( that", delimiters.ToList()))
            {
                Console.WriteLine(str);
            }
            Console.WriteLine("------------------ Finished test execution ------------------");
        }

        private string[] InclusiveSplit(string input, List<string> delimiters)
        {
            List<string> result = InclusiveSplit(input, delimiters.First()).ToList();
            delimiters.Remove(delimiters.First());

            if (delimiters.Count == 0)
                return result.ToArray();

            List<string> finalResults = new List<string>();

            if (result.Any())
            {
                for (int i = 0; i < result.Count; ++i)
                {
                    string[] temp = InclusiveSplit(result[i], new List<string>(delimiters));
                    if (temp.Any())
                        finalResults.AddRange(temp);
                }
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
                if (index != -1 && ProcessDelimiter(input, delimiter, index))
                {
                    if (index != 0)
                        output.Add(input.Substring(0, index).Trim());
                    output.Add(delimiter);
                    output = output.Concat(InclusiveSplit(input.Substring(index + delimiter.Length), delimiter)).ToList();
                    return output.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                }
                previousIndex = index + delimiter.Length;
            }
            return new string[] { input.Trim() };
        }

        private bool ProcessDelimiter(string input, string delimiter, int index)
        {
            //performs a check on the input based on the delimiter (e.g. if must have bracket after it and a space before it)
            switch (delimiter)
            {
                case "if":
                    {
                        if(index != 0)
                            if(input[index - 1] != ' ')
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
                default:
                    return true;
            }
        }
    }
}
