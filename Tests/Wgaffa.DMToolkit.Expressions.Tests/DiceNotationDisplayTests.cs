using NUnit.Framework;
using System.Collections;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;

namespace Wgaffa.DMTools.Tests
{
    [SetCulture("en-us")]
    public class DiceNotationDisplayTests
    {
        private DiceNotationDisplay _display;

        [SetUp]
        public void SetUp()
        {
            _display = new DiceNotationDisplay();
        }

        public class NumberTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(new NumberExpression(3)).Returns("3");
                yield return new TestCaseData(new NumberExpression(0)).Returns("0");
                yield return new TestCaseData(new NumberExpression(-7)).Returns("-7");
                yield return new TestCaseData(new NumberExpression(2.5f)).Returns("2.5");
                yield return new TestCaseData(new NumberExpression(10000.5f)).Returns("10000.5");
            }
        }

        public class DiceTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(new DiceExpression(Dice.d6)).Returns("d6");
                yield return new TestCaseData(new DiceExpression(Dice.d12, 3)).Returns("3d12");
                yield return new TestCaseData(new DiceExpression(Dice.d8, 3000)).Returns("3000d8");
            }
        }

        public class NegateTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new NegateExpression(new NumberExpression(3)))
                    .Returns("-3");
                yield return new TestCaseData(
                    new NegateExpression(new NumberExpression(-3)))
                    .Returns("-(-3)");
                yield return new TestCaseData(
                    new NegateExpression(
                        new NegateExpression(
                            new NumberExpression(-3))))
                    .Returns("-(-(-3))");
                yield return new TestCaseData(
                    new NegateExpression(
                        new DiceExpression(Dice.d10)))
                    .Returns("-d10");
                yield return new TestCaseData(
                    new NegateExpression(
                        new DiceExpression(Dice.d20, 3)))
                    .Returns("-3d20");
                yield return new TestCaseData(
                    new NegateExpression(
                        new AdditionExpression(
                            new NumberExpression(1),
                            new NumberExpression(2))))
                    .Returns("-(1+2)");
            }
        }

        public class AdditionTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(3), new NumberExpression(5)))
                    .Returns("3+5");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(3), new NumberExpression(-4)))
                    .Returns("3+(-4)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(-3), new NumberExpression(1)))
                    .Returns("-3+1");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NegateExpression(
                            new NumberExpression(3)),
                        new NumberExpression(7)))
                    .Returns("-3+7");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(8),
                        new NegateExpression(
                            new NumberExpression(10))))
                    .Returns("8+(-10)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NegateExpression(
                            new NumberExpression(-3)),
                        new NumberExpression(2)))
                    .Returns("-(-3)+2");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(9),
                        new NegateExpression(
                            new NumberExpression(-9))))
                    .Returns("9+(-(-9))");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new AdditionExpression(
                            new NumberExpression(-5),
                            new NumberExpression(7)),
                        new NumberExpression(-3)))
                    .Returns("-5+7+(-3)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(8),
                        new AdditionExpression(
                            new NumberExpression(-11),
                            new NumberExpression(1))))
                    .Returns("8+(-11)+1");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(1),
                        new AdditionExpression(
                            new NumberExpression(12),
                            new NumberExpression(-7))))
                    .Returns("1+12+(-7)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(1),
                        new AdditionExpression(
                            new NumberExpression(-12),
                            new NumberExpression(-7))))
                    .Returns("1+(-12)+(-7)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(1),
                        new AdditionExpression(
                            new NegateExpression(
                                new NumberExpression(15)),
                            new NumberExpression(-7))))
                    .Returns("1+(-15)+(-7)");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NegateExpression(
                            new AdditionExpression(
                                new NumberExpression(1),
                                new NumberExpression(2))),
                        new NumberExpression(3)))
                    .Returns("-(1+2)+3");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(3),
                        new NegateExpression(
                            new AdditionExpression(
                                new NumberExpression(2),
                                new NumberExpression(1)))))
                    .Returns("3+(-(2+1))");
            }
        }

        [TestCaseSource(typeof(NumberTestCaseData))]
        [TestCaseSource(typeof(DiceTestCaseData))]
        [TestCaseSource(typeof(NegateTestCaseData))]
        [TestCaseSource(typeof(AdditionTestCaseData))]
        public string NumberExpression_ToStringReturns_Expected(IExpression number)
        {
            return _display.Evaluate(number);
        }
    }
}
