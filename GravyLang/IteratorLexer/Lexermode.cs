using System;
using System.Collections.Generic;
using System.Text;

namespace GravyLang.IteratorLexer
{
    public enum LexerMode
    {
        Normal,
        Comment,
        MultiComment,
        String
    }
}
