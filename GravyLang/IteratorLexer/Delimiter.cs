using System;
using System.Collections.Generic;
using System.Text;

namespace GravyLang.IteratorLexer
{
    struct Delimiter
    {
        private string regexPreMatch;
        private string[] preMatch;
        public string toMatch;
        private string regexPostMatch;
        private string[] postMatch;

        public string RegexPreMatch
        {
            get { return regexPreMatch; }
            set
            {
                if (PreMatch != null)
                    throw new InvalidOperationException();
                regexPreMatch = value;
            }
        }

        public string[] PreMatch
        {
            get { return preMatch; }
            set
            {
                if (regexPreMatch != null)
                    throw new InvalidOperationException();
                preMatch = value;
            }
        }

        public string RegexPostMatch
        {
            get { return regexPostMatch; }
            set
            {
                if (postMatch != null)
                    throw new InvalidOperationException();
                regexPostMatch = value;
            }
        }

        public string[] PostMatch
        {
            get { return postMatch; }
            set
            {
                if (regexPostMatch != null)
                    throw new InvalidOperationException();
                postMatch = value;
            }
        }

    }

}
