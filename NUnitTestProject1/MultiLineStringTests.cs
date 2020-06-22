using GravyLang;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace LexerTests
{
    [TestFixture]
    class MultiLineStringTests
    {
        LoopStyleLexer lexer;

        [SetUp]
        public void Setup()
        {
            lexer = new LoopStyleLexer();
        }

        [Test, Category("Multi-line string")]
        public void MultiLineString()
        {
            var data = new Dictionary<string, string[]> {
                { "\"multi line", new[] { "0", "\"multi line\"", "+", "\n" } },
                {"string\"", new[] { "0", "\"string\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void FirstStringThenMultiLineString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"string\" followed by \"a multi", new[] { "0", "\"string\"", "followed", "by", "\"a multi\"", "+", "\n" } },
               {"line string\"", new[] { "0", "\"line string\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void MultiLineDirectlyBeforeNonString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"string", new[] { "0", "\"string\"", "+", "\n" } },
               {"multi-line\" something", new[] { "0", "\"multi-line\"", "something", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void StringFollowedByMultiLineString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"string\" \"something", new[] { "0", "\"string\"", "\"something\"", "+", "\n" } },
               {"multi-line\"", new[] { "0", "\"multi-line\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void MultiLineFollowedByNormalLogicThenNormalString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something", new[] { "0", "\"something\"", "+", "\n" } },
               {"multi-line\" and \"string\"", new[] { "0", "\"multi-line\"", "and", "\"string\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void tripleMultiLineString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something", new[] { "0", "\"something\"", "+", "\n" } },
               {"and another line", new[] { "0", "\"and another line\"", "+", "\n" } },
               {"end\"", new[] { "0", "\"end\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void TripleLineMultiQuoteDirectlyIntoQuote()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something", new[] { "0", "\"something\"", "+", "\n" } },
               {"and another", new[] { "0", "\"and another\"", "+", "\n" } },
               {"line\" \"quote\"", new[] { "0", "\"line\"", "\"quote\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void tripleLineMultiLineWithDelimiterThenString()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something", new[] { "0", "\"something\"", "+", "\n" } },
               {"and another", new[] { "0", "\"and another\"", "+", "\n" } },
               {"line\" and \"quote\"", new[] { "0", "\"line\"", "and", "\"quote\"", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void MultiLineStringWithoutEnd()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something", new[] { "0", "\"something\"", "+", "\n" } },
               {"and another", new[] { "0", "\"and another\"", "+", "\n" } },
               {"line quote", new[] { "0", "\"line quote\"", "+", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }

        [Test, Category("Multi-line string")]
        public void MultiLineStringWithBackslashN()
        {
            var data = new Dictionary<string, string[]> {
               { "\"something \n", new[] { "0", "\"something \n\"", "+", "\n" } },
               {"and another", new[] { "0", "\"and another\"", "+", "\n" } },
               {"line quote", new[] { "0", "\"line quote\"", "+", "\n" } }
            };

            foreach (KeyValuePair<string, string[]> InputOutput in data)
            {
                Assert.AreEqual(InputOutput.Value, lexer.Lex(InputOutput.Key));
            }
        }
    }
}
