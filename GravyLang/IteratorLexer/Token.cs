using System;
namespace GravyLang.IteratorLexer
{
    public class Token
    {
        public GravyToken Classification;
        public string Value;

        public Token(GravyToken classificiation, string value)
        {
            Classification = classificiation;
            Value = value;
        }

    }
}
