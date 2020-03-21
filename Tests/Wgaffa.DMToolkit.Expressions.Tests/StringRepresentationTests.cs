using NUnit.Framework;
using System.Collections;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMTools.Tests
{
    public class StringRepresentationTests
    {
        public class StringTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new NumberExpression(5.3))
                    .Returns("<num: 5.3>");
                yield return new TestCaseData(
                    new NumberExpression(5.255555))
                    .Returns("<num: 5.255555>");
                yield return new TestCaseData(
                    new DiceExpression(Dice.d10))
                    .Returns("<dice: rolls=1, d10>");
                yield return new TestCaseData(
                    new NegateExpression(
                        new NumberExpression(4)))
                    .Returns("<negate <num: 4>>");
                yield return new TestCaseData(
                    new AdditionExpression(
                        new DiceExpression(Dice.d4, 3),
                        new NegateExpression(
                            new NumberExpression(3.2))))
                    .Returns("<+ <dice: rolls=3, d4> <negate <num: 3.2>>>");
                yield return new TestCaseData(
                    new SubtractionExpression(
                        new NumberExpression(3),
                        new NumberExpression(1)))
                    .Returns("<- <num: 3> <num: 1>>");
                yield return new TestCaseData(
                    new MultiplicationExpression(
                        new DiceExpression(Dice.d6, 2),
                        new NumberExpression(1.5)))
                    .Returns("<* <dice: rolls=2, d6> <num: 1.5>>");
                yield return new TestCaseData(
                    new DivisionExpression(
                        new DiceExpression(Dice.d6, 6),
                        new NumberExpression(2)))
                    .Returns("</ <dice: rolls=6, d6> <num: 2>>");
                yield return new TestCaseData(
                    new VariableExpression("foo"))
                    .Returns("<var: foo>");
                yield return new TestCaseData(
                    new VariableDeclarationExpression(
                        new string[] { "foo", "bar" },
                        "int",
                        new AdditionExpression(
                            new NumberExpression(3),
                            new DiceExpression(Dice.d20))))
                    .Returns("<decl_var: type=int names=foo, bar assign=<+ <num: 3> <dice: rolls=1, d20>>>");
                yield return new TestCaseData(
                    new VariableDeclarationExpression(
                        new string[] { "foo" },
                        "real"))
                    .Returns("<decl_var: type=real names=foo>");
                yield return new TestCaseData(
                    new AssignmentExpression(
                        "foo",
                        new MultiplicationExpression(
                            new AdditionExpression(
                                new NumberExpression(5.2),
                                new NumberExpression(2.3)),
                            new NumberExpression(2))))
                    .Returns(
                    "<assign: var=foo value=<* <+ <num: 5.2> <num: 2.3>> <num: 2>>>");
            }
        }

        [TestCaseSource(typeof(StringTestCaseData))]
        public string ToString_ShouldReturnInternalRepresentation(IExpression expression)
        {
            return expression.ToString();
        }
    }
}