using GravyLang;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace LexerTests
{
    [TestFixture]
    class CommentTests
    {
        Lexer lexer;

        [SetUp]
        public void Setup()
        {
            lexer = new Lexer();
        }

        [Test, Category("comment tests")]
        public void BasicComment()
        {
            Assert.AreEqual(new string[] { "0" , "\n"}, lexer.Lex("//test string"));
        }

        [Test, Category("comment tests")]
        public void CommentMidString()
        {
            Assert.AreEqual(new string[] { "0", "\"//test string\"", "\n" }, lexer.Lex("\"//test string\""));
        }

        [Test, Category("comment tests")]
        public void CommentMidLine()
        {
            Assert.AreEqual(new string[] { "0", "something", "\n" }, lexer.Lex("something //before comment"));
        }

        [Test, Category("comment tests")]
        public void CommentMidLineStillString()
        {
            var data = new Dictionary<string, string[]> {
                { "\"multi line", new[] { "0", "\"multi line\"", "+", "\n" } },
                {"string with //comment\"", new[] { "0", "\"string with //comment\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void CommentMidLineStillStringwithOtherVariables()
        {
            var data = new Dictionary<string, string[]> {
                { "\"multi line", new[] { "0", "\"multi line\"", "+", "\n" } },
                {"string with //comment\" other", new[] { "0", "\"string with //comment\"", "other", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }
    }
}
