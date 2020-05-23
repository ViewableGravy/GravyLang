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

        [Test]
        public void DelimitersNextToEachother()
        {
            Assert.AreEqual(new string[] { "0", ")", ")", ")", ")", "\n" }, lexer.Lex("))))"));
            Assert.AreEqual(new string[] { "0", "this", "is", "\n"}, lexer.Lex("this  is  "));
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