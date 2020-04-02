using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Statements;

namespace Wgaffa.DMTools.Tests
{
    public class StringRepresentationTests
    {
        public class StringTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new Number(5.3))
                    .Returns("<num: 5.3>");
                yield return new TestCaseData(
                    new Number(5.255555))
                    .Returns("<num: 5.255555>");
                yield return new TestCaseData(
                    new DiceRoll(Dice.d10))
                    .Returns("<dice: rolls=1, d10>");
                yield return new TestCaseData(
                    new Negate(
                        new Number(4)))
                    .Returns("<negate <num: 4>>");
                yield return new TestCaseData(
                    new Addition(
                        new DiceRoll(Dice.d4, 3),
                        new Negate(
                            new Number(3.2))))
                    .Returns("<+ <dice: rolls=3, d4> <negate <num: 3.2>>>");
                yield return new TestCaseData(
                    new Subtraction(
                        new Number(3),
                        new Number(1)))
                    .Returns("<- <num: 3> <num: 1>>");
                yield return new TestCaseData(
                    new Multiplication(
                        new DiceRoll(Dice.d6, 2),
                        new Number(1.5)))
                    .Returns("<* <dice: rolls=2, d6> <num: 1.5>>");
                yield return new TestCaseData(
                    new Division(
                        new DiceRoll(Dice.d6, 6),
                        new Number(2)))
                    .Returns("</ <dice: rolls=6, d6> <num: 2>>");
                yield return new TestCaseData(
                    new Variable("foo"))
                    .Returns("<var: foo>");
                yield return new TestCaseData(
                    new VariableDeclaration(
                        new string[] { "foo", "bar" },
                        "int",
                        new Addition(
                            new Number(3),
                            new DiceRoll(Dice.d20))))
                    .Returns("<decl_var: type=int names=foo, bar assign=<+ <num: 3> <dice: rolls=1, d20>>>");
                yield return new TestCaseData(
                    new VariableDeclaration(
                        new string[] { "foo" },
                        "real"))
                    .Returns("<decl_var: type=real names=foo>");
                yield return new TestCaseData(
                    new Assignment(
                        "foo",
                        new Multiplication(
                            new Addition(
                                new Number(5.2),
                                new Number(2.3)),
                            new Number(2))))
                    .Returns(
                    "<assign: var=foo value=<* <+ <num: 5.2> <num: 2.3>> <num: 2>>>");
                yield return new TestCaseData(
                    new Definition(
                        "foo",
                        new Addition(
                            new DiceRoll(Dice.d8, 2),
                            new Number(3))))
                    .Returns("<decl_def: var=foo value=<+ <dice: rolls=2, d8> <num: 3>>>");
                yield return new TestCaseData(
                    new Drop(
                        new DiceRoll(Dice.d6, 4)))
                    .Returns("<drop: lowest <dice: rolls=4, d6>>");
                yield return new TestCaseData(
                    new Drop(
                        new DiceRoll(Dice.d6, 4), DropType.Highest))
                    .Returns("<drop: highest <dice: rolls=4, d6>>");
                yield return new TestCaseData(
                    new Keep(
                        new DiceRoll(Dice.d10, 5), 3))
                    .Returns("<keep: 3 <dice: rolls=5, d10>>");
                yield return new TestCaseData(
                    new Block(
                        new IStatement[]
                        {
                            new ExpressionStatement(
                                new Addition(
                                    new Number(3),
                                    new Number(2))),
                            new ExpressionStatement(
                                new DiceRoll(Dice.d20)),
                            new ExpressionStatement(
                                new Assignment("foo", new Number(5))),
                        }))
                    .Returns("<block: <+ <num: 3> <num: 2>> <dice: rolls=1, d20> <assign: var=foo value=<num: 5>>>");
                yield return new TestCaseData(
                    new Repeat(
                        new DiceRoll(Dice.d6, 2), 2))
                    .Returns("<repeat: 2 <dice: rolls=2, d6>>");
                yield return new TestCaseData(
                    new Function(
                        "foo",
                        new ExpressionStatement(
                            new Addition(
                                new Number(3),
                                new Number(5))),
                        "int"))
                    .Returns("<func: var=foo return=int body=<+ <num: 3> <num: 5>>>");
                yield return new TestCaseData(
                    new Function(
                        "foo",
                        new ExpressionStatement(
                            new Addition(
                                new Number(3),
                                new Number(5))),
                        "int",
                        new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("int", "a"),
                            new KeyValuePair<string, string>("int", "b")
                        }))
                    .Returns("<func: var=foo return=int body=<+ <num: 3> <num: 5>> params=int:a int:b>");
                yield return new TestCaseData(
                    new FunctionCall(
                        "foo",
                        new IExpression[]
                        {
                            new Subtraction(
                                new Number(5),
                                new Number(2))
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