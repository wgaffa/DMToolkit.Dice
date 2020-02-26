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
            }
        }

        [TestCaseSource(typeof(ValidTestCaseData))]
        public int Tokenize_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokenList = tokenizer.Tokenize(input);

            return tokenList.Count();
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
        { "dk", "5d", "d", ".5", "2d6.5" };

        [TestCaseSource(nameof(InvalidTestCaseData))]
        public void InvalidToken_ShouldThrowParseException(string input)
        {
            var tokenizer = new DiceNotationTokenizer();

            Assert.That(() => tokenizer.Tokenize(input), Throws.TypeOf<ParseException>());
        }
    }
}
