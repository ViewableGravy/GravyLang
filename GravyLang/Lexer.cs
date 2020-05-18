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

            char[] delimiters = {' ', '{', '}', '(', ')', '=', '+', '/', '-', '.', '[', ']' };

            string[] languageInputsArray = languageInput.Split(' ');

            foreach (string str in inclusiveSplit("this is: a : string that : is : meaningless :::", ':')) 
            {
                Console.WriteLine(str);
            }
            return null;
        }

        private string[] inclusiveSplit(string input, char delimiter)
        {
            List<string> output = new List<string>();
            List<int> occurences = new List<int>();

            occurences.Add(-1);
            for (int i = 0; i < input.Length; ++i)
                if(input[i] == delimiter)
                    occurences.Add(i);
            occurences.Add(input.Length);

            if(occurences.Any())
            {
                for (int i = 1; i < occurences.Count; ++i)
                {
                    output.Add(input.Substring(occurences[i - 1] + 1, occurences[i] - 1 - occurences[i - 1]));
                    if(i != occurences.Count - 1)
                        output.Add(delimiter.ToString());
                }
                return output.ToArray();
            }

            return new string[] { input };
        }

        private string[] inclusiveRecursiveSplit(string input, char[] delimiters)
        {
            foreach(char chr in input)
            {
                foreach(char del in delimiters)
                {
                    if(del == chr)
                    {
                        string[] split = input.Split(del);
                        string[] output = Array.Empty<string>();

                        //split
                        //add del in correct positions

                        foreach (string str in split) {
                            if (str != del.ToString())
                            {
                                string[] temp = inclusiveRecursiveSplit(str, delimiters.Where(val => val != del).ToArray());
                                output = output.Concat(temp).ToArray();
                            }
                        }
                        //output = output.
                        //return
                    }
                }
            }
            return new string[] { input }; 
        }
    }
}
