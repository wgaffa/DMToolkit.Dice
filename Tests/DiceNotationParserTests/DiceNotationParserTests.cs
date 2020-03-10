using NUnit.Framework;
using Superpower;
using System;
using System.Collections;
using System.Collections.Generic;
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
                yield return new TestCaseData("5 + 2d6")
                    .Returns(typeof(AdditionExpression));
                yield return new TestCaseData("7 - 3")
                    .Returns(typeof(SubtractionExpression));
                yield return new TestCaseData("2 * 3")
                    .Returns(typeof(MultiplicationExpression));
                yield return new TestCaseData("10/2")
                    .Returns(typeof(DivisionExpression));
                yield return new TestCaseData("5 * 2 + 3")
                    .Returns(typeof(AdditionExpression));
                yield return new TestCaseData("3 + 5 * 2")
                    .Returns(typeof(AdditionExpression));
                yield return new TestCaseData("-6 + 3")
                    .Returns(typeof(AdditionExpression));
                yield return new TestCaseData("7 * -3")
                    .Returns(typeof(MultiplicationExpression));
                yield return new TestCaseData("(5+3)*2")
                    .Returns(typeof(MultiplicationExpression));
                yield return new TestCaseData("2x3d4")
                    .Returns(typeof(RepeatExpression));
                yield return new TestCaseData("2x(2+3)")
                    .Returns(typeof(RepeatExpression));
                yield return new TestCaseData("[2+5, -3, 2d4 + 2]")
                    .Returns(typeof(ListExpression));
                yield return new TestCaseData("2x[5, 2d8]")
                    .Returns(typeof(RepeatExpression));
                yield return new TestCaseData("d%")
                    .Returns(typeof(DiceExpression));
                yield return new TestCaseData("2d6-L")
                    .Returns(typeof(DropExpression));
                yield return new TestCaseData("2d6-STR")
                    .Returns(typeof(SubtractionExpression));
                yield return new TestCaseData("2d6-d4")
                    .Returns(typeof(SubtractionExpression));
                yield return new TestCaseData("L-L")
                    .Returns(typeof(SubtractionExpression));
                yield return new TestCaseData("3d8 - H")
                    .Returns(typeof(DropExpression));
                yield return new TestCaseData("4d6(k3)")
                    .Returns(typeof(KeepExpression));
                yield return new TestCaseData("max(4, 2+4)")
                    .Returns(typeof(FunctionCallExpression));
                yield return new TestCaseData("5 + max(3, 2)")
                    .Returns(typeof(AdditionExpression));
            }
        }

        [TestCaseSource(typeof(ValidParseTestCaseData))]
        public Type Parse_ShouldSucceed(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var result = DiceNotationParser.Notation.Parse(tokens);

            return result.GetType();
        }

        private static readonly List<string> InvalidSyntaxTestCaseData = new List<string>()
        {
            "5 + +",
            "* 2d8",
            "5 + []",
            "[3, 13,]",
            "2x",
        };

        [TestCaseSource(nameof(InvalidSyntaxTestCaseData))]
        public void InvalidSyntax_ShouldThrowParseException(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokenlist = tokenizer.Tokenize(input);

            Assert.That(() => DiceNotationParser.Notation.Parse(tokenlist), Throws.TypeOf<ParseException>());
        }
    }
}
