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

        public class VariableTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new VariableExpression("STR"))
                    .Returns("STR");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new AdditionExpression(
                            new AdditionExpression(
                                new DiceExpression(Dice.d20),
                                new VariableExpression("BAB")),
                            new VariableExpression("STRMOD")),
                        new VariableExpression("Size")))
                    .Returns("d20+BAB+STRMOD+Size");
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
                yield return new TestCaseData(
                    new NegateExpression(
                        new SubtractionExpression(
                            new NumberExpression(5),
                            new NumberExpression(3))))
                    .Returns("-(5-3)");
                yield return new TestCaseData(
                    new NegateExpression(
                        new MultiplicationExpression(
                            new NumberExpression(2),
                            new NumberExpression(3))))
                    .Returns("-(2*3)");
                yield return new TestCaseData(
                    new NegateExpression(
                        new RepeatExpression(
                            new AdditionExpression(
                                new DiceExpression(Dice.d4),
                                new NumberExpression(3)),
                            3)))
                    .Returns("-(3x(d4+3))");
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
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NegateExpression(
                            new AdditionExpression(
                                new NumberExpression(-1),
                                new NumberExpression(7))),
                        new NumberExpression(10)))
                    .Returns("-(-1+7)+10");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NegateExpression(
                            new AdditionExpression(
                                new NumberExpression(9),
                                new NegateExpression(
                                    new AdditionExpression(
                                        new NumberExpression(-2),
                                        new NumberExpression(8))))),
                        new NumberExpression(21)))
                    .Returns("-(9+(-(-2+8)))+21");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(1),
                        new MultiplicationExpression(
                            new NumberExpression(5),
                            new NumberExpression(2))))
                    .Returns("1+5*2");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new RepeatExpression(
                            new SubtractionExpression(
                                new DiceExpression(Dice.d8, 2),
                                new NumberExpression(2)),
                            2),
                        new NumberExpression(5)
                        ))
                    .Returns("2x(2d8-2)+5");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new NumberExpression(5),
                        new RepeatExpression(
                            new MultiplicationExpression(
                                new DiceExpression(Dice.d10),
                                new NumberExpression(1.5f)),
                            2)))
                    .Returns("5+(2x(d10*1.5))");
            }
        }

        public class SubtractionTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new NumberExpression(5),
                        new NumberExpression(3)))
                    .Returns("5-3");
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new NumberExpression(-3),
                        new NumberExpression(3)))
                    .Returns("-3-3");
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new NegateExpression(
                            new SubtractionExpression(
                                new NumberExpression(10),
                                new NumberExpression(-5))),
                        new NumberExpression(-3)))
                    .Returns("-(10-(-5))-(-3)");
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new NumberExpression(10),
                        new AdditionExpression(
                            new NumberExpression(2),
                            new NumberExpression(3))))
                    .Returns("10-2+3");
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new DiceExpression(Dice.d8, 2),
                        new NumberExpression(3)))
                    .Returns("2d8-3");
            }
        }

        public class MultiplicationTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new NumberExpression(3),
                        new NumberExpression(2)))
                    .Returns("3*2");
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new NumberExpression(-3),
                        new NumberExpression(-7)))
                    .Returns("-3*(-7)");
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new AdditionExpression(
                            new NumberExpression(1),
                            new NumberExpression(5)),
                        new NumberExpression(2)))
                    .Returns("(1+5)*2");
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new NumberExpression(2),
                        new SubtractionExpression(
                            new NumberExpression(7),
                            new NumberExpression(3))))
                    .Returns("2*(7-3)");
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new NumberExpression(5),
                        new RepeatExpression(
                            new AdditionExpression(
                                new DiceExpression(Dice.d6, 2),
                                new NumberExpression(1)),
                            3)))
                    .Returns("5*(3x(2d6+1))");
            }
        }

        public class DivisionTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new DivisionExpression(
                        new NumberExpression(3),
                        new NumberExpression(2)))
                    .Returns("3/2");
                yield return new TestCaseData(
                    new DivisionExpression(
                        new NumberExpression(-3),
                        new NumberExpression(-7)))
                    .Returns("-3/(-7)");
                yield return new TestCaseData(
                    new DivisionExpression(
                        new AdditionExpression(
                            new NumberExpression(1),
                            new NumberExpression(5)),
                        new NumberExpression(2)))
                    .Returns("(1+5)/2");
                yield return new TestCaseData(
                    new DivisionExpression(
                        new NumberExpression(2),
                        new SubtractionExpression(
                            new NumberExpression(7),
                            new NumberExpression(3))))
                    .Returns("2/(7-3)");
            }
        }

        public class ListTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new ListExpression(
                        new IExpression[]
                        {
                            new NumberExpression(-3),
                            new DiceExpression(Dice.d20, 2),
                            new AdditionExpression(
                                new NumberExpression(-3),
                                new NumberExpression(3)),
                            new SubtractionExpression(
                                new DiceExpression(Dice.d8),
                                new NumberExpression(-1)),
                            new MultiplicationExpression(
                                new NumberExpression(5),
                                new AdditionExpression(
                                    new NumberExpression(1),
                                    new NumberExpression(9)))
                        }))
                    .Returns("[-3, 2d20, -3+3, d8-(-1), 5*(1+9)]");
            }
        }

        public class RepeatTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new RepeatExpression(
                        new NumberExpression(5),
                        2))
                    .Returns("2x5");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new NumberExpression(-3),
                        2))
                    .Returns("2x(-3)");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new AdditionExpression(
                            new NumberExpression(-3),
                            new NumberExpression(-3)),
                        2))
                    .Returns("2x(-3+(-3))");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new DiceExpression(Dice.d10, 3),
                        3))
                    .Returns("3x3d10");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new NegateExpression(
                            new MultiplicationExpression(
                                new DiceExpression(Dice.d4, 3),
                                new NumberExpression(2))),
                        5))
                    .Returns("5x(-(3d4*2))");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new SubtractionExpression(
                            new NumberExpression(5),
                            new NumberExpression(2))))
                    .Returns("5-2");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new ListExpression(
                            new IExpression[]
                            {
                                new NumberExpression(3),
                                new NumberExpression(-3)
                            }),
                        2))
                    .Returns("2x[3, -3]");
            }
        }

        [TestCaseSource(typeof(NumberTestCaseData))]
        [TestCaseSource(typeof(DiceTestCaseData))]
        [TestCaseSource(typeof(NegateTestCaseData))]
        [TestCaseSource(typeof(AdditionTestCaseData))]
        [TestCaseSource(typeof(SubtractionTestCaseData))]
        [TestCaseSource(typeof(MultiplicationTestCaseData))]
        [TestCaseSource(typeof(DivisionTestCaseData))]
        [TestCaseSource(typeof(ListTestCaseData))]
        [TestCaseSource(typeof(RepeatTestCaseData))]
        [TestCaseSource(typeof(VariableTestCaseData))]
        public string Evaluate_ShouldReturnString(IExpression expression)
        {
            return _display.Evaluate(expression);
        }
    }
}
