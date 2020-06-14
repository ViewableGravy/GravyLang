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
        public void BasicCommentEndOfLine()
        {
            Assert.AreEqual(new string[] { "0", "test", "\n" }, lexer.Lex("test//"));
        }

        [Test, Category("comment tests")]
        public void CommentWithStringInIt()
        {
            Assert.AreEqual(new string[] { "0", "\n" }, lexer.Lex("//\"test string\""));
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
        public void CommentMidLine2()
        {
            Assert.AreEqual(new string[] { "0", "something", "\n" }, lexer.Lex("something//before comment"));
        }

        [Test, Category("comment tests")]
        public void commentInQuoteWithSomethingOutside()
        {
            Assert.AreEqual(new[] { "0", "\"//quote\"", "else", "\n" } , lexer.Lex("\"//quote\" else"));
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
        public void multilineCommentWithOtherVariables()
        {
            var data = new Dictionary<string, string[]> {
                { "/*multi line", new[] { "0", "\n" } },
                {"comment*/ with variable", new[] { "0", "with", "variable", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void ThreeLineComment()
        {
            var data = new Dictionary<string, string[]> {
                { "/*multi line", new[] { "0", "\n" } },
                {"comment", new[] { "0", "\n" } },
                {"with 3 lines*/", new[] { "0", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void MultiLineCommentWithCommentAfterwards()
        {
            var data = new Dictionary<string, string[]> {
                { "/*multi line", new[] { "0", "\n" } },
                {"comment*/ with //comment afterwards", new[] { "0", "with", "\n" } },
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void StringIgnoreMultiLineCommentDelimiter()
        {
            var data = new Dictionary<string, string[]> {
                { "\"/*multi line", new[] { "0", "\"/*multi line\"", "+", "\n" } },
                {"comment*/\"", new[] { "0", "\"comment*/\"", "\n" } },
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void MultiLineStringDelimiterStartEndLine()
        {
            var data = new Dictionary<string, string[]> {
                { "test /*", new[] { "0", "test", "\n" } },
                {"comment*/", new[] { "0", "\n" } },
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void MultiLineCommentEndsOnNewLine()
        {
            var data = new Dictionary<string, string[]> {
                { "/*multi line", new[] { "0", "\n" } },
                {"*/ with //comment afterwards", new[] { "0", "with", "\n" } },
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("comment tests")]
        public void SlashNInComment()
        {
            var data = new Dictionary<string, string[]> {
                { "/*multi line", new[] { "0", "\n" } },
                {"\n*/", new[] { "0", "\n" } },
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }
    }
}
