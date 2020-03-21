using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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
                yield return new TestCaseData(
                    new DefinitionExpression(
                        "foo",
                        new AdditionExpression(
                            new DiceExpression(Dice.d8, 2),
                            new NumberExpression(3))))
                    .Returns("<decl_def: var=foo value=<+ <dice: rolls=2, d8> <num: 3>>>");
                yield return new TestCaseData(
                    new DropExpression(
                        new DiceExpression(Dice.d6, 4)))
                    .Returns("<drop: lowest <dice: rolls=4, d6>>");
                yield return new TestCaseData(
                    new DropExpression(
                        new DiceExpression(Dice.d6, 4), DropType.Highest))
                    .Returns("<drop: highest <dice: rolls=4, d6>>");
                yield return new TestCaseData(
                    new KeepExpression(
                        new DiceExpression(Dice.d10, 5), 3))
                    .Returns("<keep: 3 <dice: rolls=5, d10>>");
                yield return new TestCaseData(
                    new CompoundExpression(
                        new IExpression[]
                        {
                            new AdditionExpression(
                                new NumberExpression(3),
                                new NumberExpression(2)),
                            new DiceExpression(Dice.d20),
                            new AssignmentExpression("foo", new NumberExpression(5)),
                        }))
                    .Returns("<block: <+ <num: 3> <num: 2>> <dice: rolls=1, d20> <assign: var=foo value=<num: 5>>>");
                yield return new TestCaseData(
                    new RepeatExpression(
                        new DiceExpression(Dice.d6, 2), 2))
                    .Returns("<repeat: 2 <dice: rolls=2, d6>>");
                yield return new TestCaseData(
                    new FunctionExpression(
                        "foo",
                        new AdditionExpression(
                            new NumberExpression(3),
                            new NumberExpression(5)),
                        "int"))
                    .Returns("<func: var=foo return=int body=<+ <num: 3> <num: 5>>>");
                yield return new TestCaseData(
                    new FunctionExpression(
                        "foo",
                        new AdditionExpression(
                            new NumberExpression(3),
                            new NumberExpression(5)),
                        "int",
                        new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("int", "a"),
                            new KeyValuePair<string, string>("int", "b")
                        }))
                    .Returns("<func: var=foo return=int body=<+ <num: 3> <num: 5>> params=int:a int:b>");
                yield return new TestCaseData(
                    new FunctionCallExpression(
                        "foo",
                        new IExpression[]
                        {
                            new SubtractionExpression(
                                new NumberExpression(5),
                                new NumberExpression(2))
                        }))
                    .Returns("<call: foo args=<- <num: 5> <num: 2>>>");
            }
        }

        [TestCaseSource(typeof(StringTestCaseData))]
        public string ToString_ShouldReturnInternalRepresentation(IExpression expression)
        {
            return expression.ToString();
        }
    }
}