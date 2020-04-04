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
        public class ExpressionStringTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new Literal(5.3))
                    .Returns("<literal: 5.3>");
                yield return new TestCaseData(
                    new Literal(5.255555))
                    .Returns("<literal: 5.255555>");
                yield return new TestCaseData(
                    new DiceRoll(Dice.d10))
                    .Returns("<dice: rolls=1, d10>");
                yield return new TestCaseData(
                    new Negate(
                        new Literal(4)))
                    .Returns("<negate <literal: 4>>");
                yield return new TestCaseData(
                    new Addition(
                        new DiceRoll(Dice.d4, 3),
                        new Negate(
                            new Literal(3.2))))
                    .Returns("<+ <dice: rolls=3, d4> <negate <literal: 3.2>>>");
                yield return new TestCaseData(
                    new Subtraction(
                        new Literal(3),
                        new Literal(1)))
                    .Returns("<- <literal: 3> <literal: 1>>");
                yield return new TestCaseData(
                    new Multiplication(
                        new DiceRoll(Dice.d6, 2),
                        new Literal(1.5)))
                    .Returns("<* <dice: rolls=2, d6> <literal: 1.5>>");
                yield return new TestCaseData(
                    new Division(
                        new DiceRoll(Dice.d6, 6),
                        new Literal(2)))
                    .Returns("</ <dice: rolls=6, d6> <literal: 2>>");
                yield return new TestCaseData(
                    new Variable("foo"))
                    .Returns("<var: foo>");
                yield return new TestCaseData(
                    new Assignment(
                        "foo",
                        new Multiplication(
                            new Addition(
                                new Literal(5.2),
                                new Literal(2.3)),
                            new Literal(2))))
                    .Returns(
                    "<assign: var=foo value=<* <+ <literal: 5.2> <literal: 2.3>> <literal: 2>>>");
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
                    new Repeat(
                        new DiceRoll(Dice.d6, 2), 2))
                    .Returns("<repeat: 2 <dice: rolls=2, d6>>");
                yield return new TestCaseData(
                    new FunctionCall(
                        "foo",
                        new IExpression[]
                        {
                            new Subtraction(
                                new Literal(5),
                                new Literal(2))
                        }))
                    .Returns("<call: foo args=<- <literal: 5> <literal: 2>>>");
            }
        }

        public class StatementStringTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new VariableDeclaration(
                        new string[] { "foo", "bar" },
                        "int",
                        new Addition(
                            new Literal(3),
                            new DiceRoll(Dice.d20))))
                    .Returns("<decl_var: type=int names=foo, bar assign=<+ <literal: 3> <dice: rolls=1, d20>>>");
                yield return new TestCaseData(
                    new VariableDeclaration(
                        new string[] { "foo" },
                        "real"))
                    .Returns("<decl_var: type=real names=foo>");
                yield return new TestCaseData(
                    new Definition(
                        "foo",
                        new Addition(
                            new DiceRoll(Dice.d8, 2),
                            new Literal(3))))
                    .Returns("<decl_def: var=foo value=<+ <dice: rolls=2, d8> <literal: 3>>>");
                yield return new TestCaseData(
                    new Block(
                        new IStatement[]
                        {
                            new ExpressionStatement(
                                new Addition(
                                    new Literal(3),
                                    new Literal(2))),
                            new ExpressionStatement(
                                new DiceRoll(Dice.d20)),
                            new ExpressionStatement(
                                new Assignment("foo", new Literal(5))),
                        }))
                    .Returns("<block: <+ <literal: 3> <literal: 2>> <dice: rolls=1, d20> <assign: var=foo value=<literal: 5>>>");
                yield return new TestCaseData(
                    new Function(
                        "foo",
                        new ExpressionStatement(
                            new Addition(
                                new Literal(3),
                                new Literal(5))),
                        "int"))
                    .Returns("<func: var=foo return=int body=<+ <literal: 3> <literal: 5>>>");
                yield return new TestCaseData(
                    new Function(
                        "foo",
                        new ExpressionStatement(
                            new Addition(
                                new Literal(3),
                                new Literal(5))),
                        "int",
                        new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("int", "a"),
                            new KeyValuePair<string, string>("int", "b")
                        }))
                    .Returns("<func: var=foo return=int body=<+ <literal: 3> <literal: 5>> params=int:a int:b>");
            }
        }

        [TestCaseSource(typeof(ExpressionStringTestCaseData))]
        public string ToString_ShouldReturnInternalRepresentation(IExpression expression)
        {
            return expression.ToString();
        }

        [TestCaseSource(typeof(StatementStringTestCaseData))]
        public string ToString_ShouldReturnInternalRepresentation_GivenStatement(IStatement statement)
        {
            return statement.ToString();
        }
    }
}