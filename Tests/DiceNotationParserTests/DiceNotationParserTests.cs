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
            }
        }

        [TestCaseSource(typeof(ValidParseTestCaseData))]
        public Type Parse_ShouldSucceed(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var result = DiceNotationParser.Lambda.Parse(tokenizer.Tokenize(input));

            return result.GetType();
        }
    }
}
