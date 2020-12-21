using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GravyLang.IteratorLexer
{
    /// <summary>
    /// A class used to determine the correlating token for an item
    /// </summary>
    public class Tokenizer
    {
        private string[] keywords = new[]
        {
            "for",
            "foreach",
            "if",
            "else",
            "while",
            "do",
            "and",
            "this"
        };
        private string[] types = new[]
        {
            "int",
            "string",
            "char",
            "bool",
            "var"
        };
        private string[] separators = new[]
        {
            @"\n",
            @"\r",
            "(",
            ")",
            ";",
            ":",
            "{",
            "}",
            ",",
            "[",
            "]"
        };
        private string[] operaters = new[]
        {

            "!=",
            "==",
            "+",
            "-",
            "/",
            "*",
            "%",
            "=",
            "?"
        };
        private string[] literal = new[]
        {
            "true",
            "false",
            @"^[0-9]+$", //integer literal
            @"^[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)$", //number literal
            "^\"" //string literal
        };

        public GravyToken GetToken(string str)
        {
            return keywords.Contains(str)   ? GravyToken.keyword   :
                   types.Contains(str)      ? GravyToken.type      :
                   separators.Contains(str) ? GravyToken.separator :
                   operaters.Contains(str)  ? GravyToken.operater  :
                   IsLiteral(str)           ? GravyToken.literal   :
                   GravyToken.identifier;
        }

        private bool IsLiteral(string str)
        {
            foreach(string literalMatcher in literal)
                if(Regex.IsMatch(str, literalMatcher))
                    return true;
            return false;
        }
    }
}
