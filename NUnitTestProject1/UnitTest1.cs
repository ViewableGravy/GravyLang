using GravyLang;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace LexerTests
{
    [TestFixture]
    public class LexerTests
    {
        Lexer lexer;

        [SetUp]
        public void Setup()
        {
            lexer = new Lexer();
        }

        [Test]
        public void delimiterTouchingNonDelimiter()
        {
            Assert.AreEqual(new string[] { "0", ")", "and", "is", ")", "\n" }, lexer.Lex(")and is)"));
        }

        [TestCase(new[] { "0", ")", ")", ")", ")", "\n" }, "))))")]
        [TestCase(new[] { "0", ":", ":", "something", ":", ":", "\n" }, "::something::")]
        [TestCase(new[] { "0", "this", "is", "\n" }, "this  is  ")]
        public void DelimitersNextToEachother(string[] output, string input)
        {
            Assert.AreEqual(output, lexer.Lex(input));
        }

        [Test]
        public void multipleDelimiters()
        {
            Assert.AreEqual(new string[] { "0", ":", "other", ")", ":", "and", "\n" }, lexer.Lex(":other):and"));
        }

        [TestCase(new[] { "0", "\n" }, "  ")]
        [TestCase(new[] { "0", "\n" }, "")]
        [TestCase(new[] { "2", "test", "\n" }, "  test")]
        [TestCase(new[] { "4", "test", "\n" }, "    test")]
        public void indentation(string[] output, string input)
        {
            Assert.AreEqual(output, lexer.Lex(input));
        }

        [Test]
        public void touchingIfDelimiter()
        {
            Assert.AreEqual(new string[] { "0", "(", ")", "ifelse", "if", "\n" }, lexer.Lex("()ifelse if"));
        }

        [Test]
        public void MultipleStrings()
        {
            Assert.AreEqual(new string[] { "0", "\"white space\"", ".", "\"more space\"", "\n" }, lexer.Lex("\"white space\" . \"more space\""));
        }

        [Test]
        public void MultipleQuotes()
        {
            Assert.AreEqual(new string[] { "0", "\"\"", "\"\"", "\n" }, lexer.Lex("\"\"\"\""));
        }

        [Test]
        public void MultipleQuotesOdd()
        {
            Assert.AreEqual(new string[] { "0", "\"\"", "\"\"", "\"", "+", "\n" }, lexer.Lex("\"\"\"\"\""));
        }

        [Test]
        public void StringsWithoutSecondQuote()
        {
            Assert.AreEqual(new string[] { "0", "\"white space\"", "+", "\n" }, lexer.Lex("\"white space"));
        }

        [TestCase(new[] { "0", "elseif", "if", "\n" }, "elseif if")]
        [TestCase(new[] { "0", "else", "if", "\n" }, "else if")]
        public void elseIfDelimiter(string[] output, string input)
        {
            Assert.AreEqual(output, lexer.Lex(input));
        }

        [Test]
        public void doubleElseInName()
        {
            Assert.AreEqual(new string[] { "0", "elseelse", "\n" }, lexer.Lex("elseelse"));
        }

        [Test]
        public void SlashNInLine()
        {
            Assert.AreEqual(new string[] { "0", "\n", "\n" }, lexer.Lex("\n"));
        }


    }
}