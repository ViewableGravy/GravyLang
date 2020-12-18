using System;
using System.Collections;

namespace GravyLang.IteratorLexer
{
    public class Itr : IEnumerator, IEnumerable
    {
        private string line;
        private LexerMode mode;
        private int index = -1;
        private bool multiLineString = false;
        private bool multiLineComment = false;

        public LexerMode Mode
        {
            get => mode;
            set
            {
                if (value == LexerMode.Comment && this.mode == LexerMode.String)
                    throw new InvalidOperationException();
                mode = value;
            }
        }
        public int Index { get => index; }
        public string Line
        {
            set
            {
                if (line != null && Index != line.Length && index != -1)
                    throw new InvalidOperationException("Line was not yet finished processing");
                line = value;
            }
        }

        public bool MoveNext()
        {
            index++;
            return (index < line.Length);
        }

        public void Reset()
        {
            index = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return line[index];
            }
        }

        public bool MultiLineString
        {
            get => multiLineString;
            set
            {
                if (multiLineComment)
                    throw new InvalidOperationException("Cannot have multi-line string/comment at the same time");
                multiLineString = value;
            }
        }

        public bool MultiLineComment
        {
            get => multiLineComment;
            set
            {
                if (multiLineString)
                    throw new InvalidOperationException("Cannot have multi-line string/comment at the same time");
                multiLineComment = value;
            }
        }

        public char Current()
        {
            return line[index];
        }

        public IEnumerator GetEnumerator()
        {
            foreach (char c in line)
            {
                yield return c;
            }
        }

        public char Peek(int i)
        {
            if (index + i < line.Length && index + i >= 0)
                return line[index + i];
            throw new IndexOutOfRangeException();
        }

        public bool PeekAndEquals(int i, string str)
        {
            if (index + i + str.Length > this.line.Length || index + i < 0)
                return false;
            if (str[0] != this.Peek(i))
                return false;
            for (int j = 1; j < str.Length; ++j)
                if (!str[j].Equals(this.Peek(i + j)))
                    return false;
            return true;
        }

        public static bool operator ==(Itr itr, string str)
        {
            return itr.PeekAndEquals(0, str);
        }

        public static bool operator !=(Itr itr, string str)
        {
            return !(itr == str);
        }

        public bool IsFinalIndex()
        {
            return index == line.Length - 1;
        }

        public bool IsFirstIndex()
        {
            return index == 0;
        }
    }
}
