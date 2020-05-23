using GravyLang;
using NUnit.Framework;

namespace NUnitTestProject1
{
    public class Tests
    {
        Lexer lexer;
        [SetUp]
        public void Setup()
        {
            lexer = new Lexer();
        }

        [Test, MaxTime(2000)]
        public void DelimitersNextToEachother()
        {
            Assert.AreEqual(new string[] { "0", ")", ")", ")", ")", "\n" }, lexer.Lex("))))"));


            /*
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
            Assert.Pass();
            */
        }

        [Test]
        public void delimiterTouchingNonDelimiter()
        {
            Assert.AreEqual(new string[] { "0", ")", "and", "is", ")", "\n" }, lexer.Lex(")and is)"));
        }

        [Test]
        public void multipleDelimiters()
        {
            Assert.AreEqual(new string[] { "0", ":", "other", ")", ":", "and", "\n" }, lexer.Lex(":other):and"));
        }

        [Test]
        public void indentation()
        {
            Assert.AreEqual(new string[] { "0","\n" }, lexer.Lex("  "));
            Assert.AreEqual(new string[] { "0", "\n" }, lexer.Lex(""));
            Assert.AreEqual(new string[] { "2", "test", "\n" }, lexer.Lex("  test"));
            Assert.AreEqual(new string[] { "4", "test", "\n" }, lexer.Lex("    test"));
        }

        [Test]
        public void touchingIfDelimiter()
        {
            Assert.AreEqual(new string[] { "0", "(", ")", "ifelse", "if", "\n" }, lexer.Lex("()ifelse if"));
        }

        [Test]
        public void elseIfDelimiter()
        {
            Assert.AreEqual(new string[] { "0", "elseif", "if", "\n" }, lexer.Lex("elseif if"));
            Assert.AreEqual(new string[] { "0", "else", "if", "\n" }, lexer.Lex("else if"));
        }

        [Test]
        public void doubleElseInName()
        {
            Assert.AreEqual(new string[] { "0", "elseelse", "\n" }, lexer.Lex("elseelse"));
        }
    }
}