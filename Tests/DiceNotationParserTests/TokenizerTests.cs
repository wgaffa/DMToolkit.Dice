using NUnit.Framework;
using Superpower;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    [TestFixture]
    public class TokenizerTests
    {
        public class ValidTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("5").Returns(1);
                yield return new TestCaseData("  5  ").Returns(1);
                yield return new TestCaseData("2d6").Returns(1);
                yield return new TestCaseData("d12").Returns(1);
                yield return new TestCaseData("5.2").Returns(1);
                yield return new TestCaseData("+ 3 -2 * /").Returns(6);
                yield return new TestCaseData("+ 3.2 -7 * /").Returns(6);
                yield return new TestCaseData("3+5").Returns(3);
                yield return new TestCaseData("()").Returns(2);
                yield return new TestCaseData("2x").Returns(1);
                yield return new TestCaseData("[]").Returns(2);
                yield return new TestCaseData(",").Returns(1);
                yield return new TestCaseData("L").Returns(1);
                yield return new TestCaseData("STR").Returns(1);
                yield return new TestCaseData("L-L").Returns(3);
                yield return new TestCaseData("drone").Returns(1);
                yield return new TestCaseData(";").Returns(1);
                yield return new TestCaseData(":").Returns(1);
                yield return new TestCaseData("=").Returns(1);
                yield return new TestCaseData("3.2452").Returns(1);
            }
        }

        [TestCaseSource(typeof(ValidTestCaseData))]
        public int Tokenize_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokenList = tokenizer.Tokenize(input);
            return tokenList.Count();
        }


        // Read more https://github.com/nunit/docs/wiki/TestCaseSource-Attribute and https://github.com/nunit/docs/wiki/TestCaseData

        public class TokenKindTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("2.3")
                    .Returns(DiceNotationToken.Number);
                yield return new TestCaseData("542")
                    .Returns(DiceNotationToken.Number);
                yield return new TestCaseData("-")
                    .Returns(DiceNotationToken.Minus);
                yield return new TestCaseData("+")
                    .Returns(DiceNotationToken.Plus);
                yield return new TestCaseData("*")
                    .Returns(DiceNotationToken.Multiplication);
                yield return new TestCaseData("/")
                    .Returns(DiceNotationToken.Divide);
                yield return new TestCaseData("foo")
                    .Returns(DiceNotationToken.Identifier);
                yield return new TestCaseData("=")
                    .Returns(DiceNotationToken.Equal);
                yield return new TestCaseData(";")
                    .Returns(DiceNotationToken.SemiColon);
                yield return new TestCaseData(":")
                    .Returns(DiceNotationToken.Colon);
                yield return new TestCaseData("(")
                    .Returns(DiceNotationToken.LParen);
                yield return new TestCaseData(")")
                    .Returns(DiceNotationToken.RParen);
                yield return new TestCaseData("[")
                    .Returns(DiceNotationToken.LBracket);
                yield return new TestCaseData("]")
                    .Returns(DiceNotationToken.RBracket);
                yield return new TestCaseData("2d6")
                    .Returns(DiceNotationToken.Dice);
                yield return new TestCaseData("d20")
                    .Returns(DiceNotationToken.Dice);
                yield return new TestCaseData("3x")
                    .Returns(DiceNotationToken.Repeat);
                yield return new TestCaseData("def")
                    .Returns(DiceNotationToken.Keyword);
                yield return new TestCaseData("end")
                    .Returns(DiceNotationToken.Keyword);
            }
        }

        [TestCaseSource(typeof(TokenKindTestCaseData))]
        public DiceNotationToken Tokenize_ReturnsCorrectToken(string input)
        {
            return new DiceNotationTokenizer().Tokenize(input).Single().Kind;
        }

        public class ArithmeticTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("1 + 5").Returns(3);
                yield return new TestCaseData("2.4 - 2").Returns(3);
                yield return new TestCaseData("10004.2 / 100").Returns(3);
                yield return new TestCaseData("5 * 1.5").Returns(3);
                yield return new TestCaseData("1 + 5 - 2 * 3 / 2").Returns(9);
            }
        }

        [TestCaseSource(typeof(ArithmeticTestCaseData))]
        public int ArithmeticTokenize_ReturnsCorrectNumberOfTokens(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokens = tokenizer.Tokenize(input);

            return tokens.Count();
        }

        private static readonly List<string> DiceTokenizeTestCaseData = new List<string>
        { "d20", "d8", "d4", "d6", "3d8", "2d12", "10d3", };

        [TestCaseSource(nameof(DiceTokenizeTestCaseData))]
        public void DiceTokenize_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokens = tokenizer.Tokenize(input);

            Assert.That(tokens.Count, Is.EqualTo(1));
        }

        private static readonly List<string> InvalidWhitespaceTestCaseData = new List<string>
        {
            "10d20d6", "d65d10", "5.5d4"
        };

        [TestCaseSource(nameof(InvalidWhitespaceTestCaseData))]
        public void Tokenize_ShouldThrowParseException_GivenInvalidWhitespace(string input)
        {
            var tokenizer = new DiceNotationTokenizer();

            Assert.That(() => tokenizer.Tokenize(input), Throws.TypeOf<ParseException>());
        }

        private static readonly List<string> InvalidTestCaseData = new List<string>
        { "5d", ".5", "2d6.5" };

        [TestCaseSource(nameof(InvalidTestCaseData))]
        public void InvalidToken_ShouldThrowParseException(string input)
        {
            var tokenizer = new DiceNotationTokenizer();

            Assert.That(() => tokenizer.Tokenize(input), Throws.TypeOf<ParseException>());
        }
    }
}
