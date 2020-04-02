using NUnit.Framework;
using Superpower;
using System;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.DMToolkit.Statements;

namespace DiceNotationParserTests
{
    public class DiceNotationParserTests
    {
        public class ValidParseTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("5")
                    .Returns(typeof(Number));
                yield return new TestCaseData("5.2")
                    .Returns(typeof(Number));
                yield return new TestCaseData("3d8")
                    .Returns(typeof(DiceRoll));
                yield return new TestCaseData("d6")
                    .Returns(typeof(DiceRoll));
                yield return new TestCaseData("5 + 2d6")
                    .Returns(typeof(Addition));
                yield return new TestCaseData("7 - 3")
                    .Returns(typeof(Subtraction));
                yield return new TestCaseData("2 * 3")
                    .Returns(typeof(Multiplication));
                yield return new TestCaseData("10/2")
                    .Returns(typeof(Division));
                yield return new TestCaseData("5 * 2 + 3")
                    .Returns(typeof(Addition));
                yield return new TestCaseData("3 + 5 * 2")
                    .Returns(typeof(Addition));
                yield return new TestCaseData("-6 + 3")
                    .Returns(typeof(Addition));
                yield return new TestCaseData("7 * -3")
                    .Returns(typeof(Multiplication));
                yield return new TestCaseData("(5+3)*2")
                    .Returns(typeof(Multiplication));
                yield return new TestCaseData("2x3d4")
                    .Returns(typeof(Repeat));
                yield return new TestCaseData("2x(2+3)")
                    .Returns(typeof(Repeat));
                yield return new TestCaseData("[2+5, -3, 2d4 + 2]")
                    .Returns(typeof(ListExpression));
                yield return new TestCaseData("2x[5, 2d8]")
                    .Returns(typeof(Repeat));
                yield return new TestCaseData("d%")
                    .Returns(typeof(DiceRoll));
                yield return new TestCaseData("2d6-L")
                    .Returns(typeof(Drop));
                yield return new TestCaseData("2d6-STR")
                    .Returns(typeof(Subtraction));
                yield return new TestCaseData("2d6-d4")
                    .Returns(typeof(Subtraction));
                yield return new TestCaseData("L-L")
                    .Returns(typeof(Subtraction));
                yield return new TestCaseData("3d8 - H")
                    .Returns(typeof(Drop));
                yield return new TestCaseData("4d6(k3)")
                    .Returns(typeof(Keep));
                yield return new TestCaseData("max(4, 2+4)")
                    .Returns(typeof(FunctionCall));
                yield return new TestCaseData("5 + max(3, 2)")
                    .Returns(typeof(Addition));
                yield return new TestCaseData("real foo;")
                    .Returns(typeof(VariableDeclaration));
                yield return new TestCaseData("real foo; int bar;")
                    .Returns(typeof(Block));
                yield return new TestCaseData("foo = 6")
                    .Returns(typeof(Assignment));
                yield return new TestCaseData("real foo, bar;")
                    .Returns(typeof(VariableDeclaration));
                yield return new TestCaseData("int foo = 5;")
                    .Returns(typeof(VariableDeclaration));
                yield return new TestCaseData("def StandardRoll = 10 + d6;")
                    .Returns(typeof(Definition));
                yield return new TestCaseData("int Test(int a, real b) a + b; end")
                    .Returns(typeof(Function));
                yield return new TestCaseData("real foo = 5; real Bar() 5; end")
                    .Returns(typeof(Block));
                yield return new TestCaseData(@"""Some form of string""")
                    .Returns(typeof(Wgaffa.DMToolkit.Expressions.String));
                yield return new TestCaseData(@"""Another \""quoted\"" string""")
                    .Returns(typeof(Wgaffa.DMToolkit.Expressions.String));
            }
        }

        [TestCaseSource(typeof(ValidParseTestCaseData))]
        public Type Parse_ShouldSucceed(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var result = DiceNotationParser.Program.Parse(tokens);

            return result.GetType();
        }

        private static readonly List<string> InvalidSyntaxTestCaseData = new List<string>()
        {
            "5 + +",
            "* 2d8",
            "5 + []",
            "[3, 13,]",
            "2x",
            "real foo; int bar",
            "def = 5 + 5;",
            "def Bee =",
            "def A = 5 + def",
        };

        [TestCaseSource(nameof(InvalidSyntaxTestCaseData))]
        public void InvalidSyntax_ShouldThrowParseException(string input)
        {
            var tokenizer = new DiceNotationTokenizer();
            var tokenlist = tokenizer.Tokenize(input);

            Assert.That(() => DiceNotationParser.Program.Parse(tokenlist), Throws.TypeOf<ParseException>());
        }
    }
}
