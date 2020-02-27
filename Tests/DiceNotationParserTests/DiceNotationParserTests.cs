using NUnit.Framework;
using Superpower;
using System;
using System.Collections;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    public class DiceNotationParserTests
    {
        public class ValidParseTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("5")
                    .Returns(typeof(NumberExpression));
                yield return new TestCaseData("5.2")
                    .Returns(typeof(NumberExpression));
                yield return new TestCaseData("3d8")
                    .Returns(typeof(DiceExpression));
                yield return new TestCaseData("d6")
                    .Returns(typeof(DiceExpression));
            }
        }

        [TestCaseSource(typeof(ValidParseTestCaseData))]
        public Type Parse_ShouldSucceed(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var result = DiceNotationParser.Notation.Parse(tokens);

            return result.GetType();
        }
    }
}
