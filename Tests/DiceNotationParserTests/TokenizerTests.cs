using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    [TestFixture]
    public class TokenizerTests
    {
        [TestCase("5")]
        [TestCase("5.5")]
        [TestCase("   \t7.5")]
        public void Tokenize_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokenList = tokenizer.Tokenize(input);

            Assert.That(tokenList.Count(), Is.EqualTo(1));
        }


        // Read more https://github.com/nunit/docs/wiki/TestCaseSource-Attribute and https://github.com/nunit/docs/wiki/TestCaseData

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

        static readonly List<string> DiceTokenizeTestCaseData = new List<string>
        {
            "d20",
            "d8",
            "d4",
            "d6",
            "3d8",
            "2d12",
            "10d3",
        };

        [TestCaseSource(nameof(DiceTokenizeTestCaseData))]
        public void DiceTokenize_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokens = tokenizer.Tokenize(input);

            Assert.That(tokens.Count, Is.EqualTo(1));
        }
    }
}
