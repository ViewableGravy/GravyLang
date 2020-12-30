using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GravyLang.IteratorLexer
{
    public class MultiLineLexer
    {
        IteratorLexer iterator = new IteratorLexer();
        Tokenizer tokenizer = new Tokenizer();

        public IEnumerable LexFile(string file)
        {
            
            StreamReader streamReader = new StreamReader(file);
            string line = "";
            while ((line = streamReader.ReadLine()) != null)
                foreach(string str in iterator.GenerateLexemes(line))
                    yield return new Token(tokenizer.GetToken(str), str);
        }
    }

    

}
