using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GravyLang
{
    class Lexer
    {
        

        public string[] Lex(string languageInput)
        {
            
            char[] delimiters = { '{', '}', '(', ')', '=', '+', '/', '-', '.', '[', ']' };
            char[] temp = {':', '}'};

            Console.WriteLine("Test case 1");
            foreach (string str in inclusiveSplit("}}}", '}'))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case 2");
            foreach (string str in inclusiveSplit("this is:} a : string that :} is : meaningless :::", ':'))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case 3");
            foreach (string str in inclusiveSplit("} a", '}'))
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Test case recursive");
            foreach (string str in inclusiveRecursiveSplit("this is:} a : string } that :} is : meaningless :::", temp.ToList())) 
            {
                Console.WriteLine(str);
            }
            return null;
        }

        private string[] inclusiveSplit(string input, char delimiter)
        {
            List<string> output = new List<string>();

            int previousInstance = 0;
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == delimiter)
                {
                    if (previousInstance == 0)
                    {
                        if (i == 0)
                        {
                            output.Add(delimiter.ToString());
                            previousInstance = i + 1;
                        }
                        else
                        {
                            output.Add(input.Substring(0, i).Trim());
                            output.Add(delimiter.ToString());
                            previousInstance = i + 1;
                        }
                    }
                    else
                    {
                        if (i != previousInstance)
                            output.Add(input.Substring(previousInstance, i - previousInstance).Trim());
                        output.Add(delimiter.ToString());
                        previousInstance = i + 1;
                    }
                }
                else
                {
                    //last index
                    if (i == input.Length - 1 && i > previousInstance)
                    {
                        if (previousInstance != 0)
                        {
                            output.Add(input.Substring(previousInstance, i + 1 - previousInstance).Trim());
                        }
                        else
                        {
                            return new string[] { input };
                        }
                    }
                }
            }

            if(output.Any())
                return output.ToArray();

            return new string[] { input };
        }

        private string[] inclusiveRecursiveSplit(string input, List<char> delimiters)
        {
            List<string> result = inclusiveSplit(input, delimiters.First()).ToList();
            delimiters.Remove(delimiters.First());

            if(delimiters.Count == 0)
                return result.ToArray();

            List<string> finalResults = new List<string>();

            if (result.Any())
            {
                for (int i = 0; i < result.Count; ++i)
                {
                    string[] temp = inclusiveRecursiveSplit(result[i], new List<char>(delimiters));
                    if (temp.Any())
                        finalResults.AddRange(temp);
                }
            }

            return finalResults.ToArray();
        }
    }
}
