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
                yield return new TestCaseData("5;");
                yield return new TestCaseData("5.2;");
                yield return new TestCaseData("3d8;");
                yield return new TestCaseData("d6;");
                yield return new TestCaseData("5 + 2d6;");
                yield return new TestCaseData("7 - 3;");
                yield return new TestCaseData("2 * 3;");
                yield return new TestCaseData("10/2;");
                yield return new TestCaseData("5 * 2 + 3;");
                yield return new TestCaseData("3 + 5 * 2;");
                yield return new TestCaseData("-6 + 3;");
                yield return new TestCaseData("7 * -3;");
                yield return new TestCaseData("(5+3)*2;");
                yield return new TestCaseData("2x3d4;");
                yield return new TestCaseData("2x(2+3);");
                yield return new TestCaseData("[2+5, -3, 2d4 + 2];");
                yield return new TestCaseData("2x[5, 2d8];");
                yield return new TestCaseData("d%;");
                yield return new TestCaseData("2d6-L;");
                yield return new TestCaseData("2d6-STR;");
                yield return new TestCaseData("2d6-d4;");
                yield return new TestCaseData("L-L;");
                yield return new TestCaseData("3d8 - H;");
                yield return new TestCaseData("4d6(k3);");
                yield return new TestCaseData("max(4, 2+4);");
                yield return new TestCaseData("5 + max(3, 2);");
                yield return new TestCaseData("real foo;");
                yield return new TestCaseData("real foo; int bar;");
                yield return new TestCaseData("foo = 6;");
                yield return new TestCaseData("real foo, bar;");
                yield return new TestCaseData("int foo = 5;");
                yield return new TestCaseData("def StandardRoll = 10 + d6;");
                yield return new TestCaseData("int Test(int a, real b) a + b; end");
                yield return new TestCaseData("real foo = 5; real Bar() 5; end");
                yield return new TestCaseData(@"""Some form of string"";");
                yield return new TestCaseData(@"""Another \""quoted\"" string"";");
                yield return new TestCaseData("return;");
                yield return new TestCaseData("return 5+2d6;");
            }
        }

        [TestCaseSource(typeof(ValidParseTestCaseData))]
        public void Parse_ShouldSucceed(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var result = DiceNotationParser.Program.Parse(tokens);

            Assert.That(result, Is.Not.Null);
        }

        private static readonly List<string> InvalidSyntaxTestCaseData = new List<string>()
        {
            "5 + +;",
            "* 2d8;",
            "5 + [];",
            "[3, 13,];",
            "2x;",
            "real foo; int bar",
            "def = 5 + 5;",
            "def Bee =;",
            "def A = 5 + def;",
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
