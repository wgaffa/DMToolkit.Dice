using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Exceptions;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    [TestFixture]
    public class DiceNotationInterpreterTests
    {
        private Mock<ISymbol> _mockSymbol;
        private Mock<ISymbolTable> _symbolTable;
        private DiceNotationInterpreter _interpreter;
        private Mock<IDiceRoller> _mockRoller;

        [SetUp]
        public void Setup()
        {
            _mockRoller = new Mock<IDiceRoller>();
            _mockRoller.SetupSequence(d => d.RollDice(It.IsAny<Dice>()))
                .Returns(2)
                .Returns(4)
                .Returns(2)
                .Returns(6)
                .Returns(1);

            _mockSymbol = new Mock<ISymbol>();
            _symbolTable = new Mock<ISymbolTable>();
            _symbolTable.Setup(x => x.Lookup("BAB"))
                .Returns(new VariableSymbol("BAB", Maybe<ISymbol>.Some(_mockSymbol.Object)));
            _symbolTable.Setup(x => x.Lookup("StrMod"))
                .Returns(new VariableSymbol("StrMod", Maybe<ISymbol>.Some(_mockSymbol.Object)));
            _symbolTable.Setup(x => x.Lookup("Size"))
                .Returns(new VariableSymbol("Size", Maybe<ISymbol>.Some(_mockSymbol.Object)));

            _interpreter = new DiceNotationInterpreter();
        }

        public class VariableTestCaseData : IEnumerable
        {
            private readonly Mock<IDiceRoller> _mockRoller;

            public VariableTestCaseData()
            {
                _mockRoller = new Mock<IDiceRoller>();

                _mockRoller
                    .Setup(x => x.RollDice(It.IsAny<Dice>()))
                    .Returns(4);
            }

            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    new AdditionExpression(
                        new AdditionExpression(
                            new AdditionExpression(
                                new DiceExpression(_mockRoller.Object, Dice.d20),
                                new VariableExpression("BAB")),
                            new VariableExpression("StrMod")),
                        new VariableExpression("Size")))
                    .Returns(11);
            }
        }

        [TestCaseSource(typeof(VariableTestCaseData))]
        public double Evaluate_ShouldReturnResult(IExpression expression)
        {
            var context = new DiceNotationContext(expression)
            {
                SymbolTable = _symbolTable.Object
            };

            var setup = new Dictionary<string, double>
            {
                ["BAB"] = 4,
                ["Size"] = -1,
                ["StrMod"] = 4,
            };

            return _interpreter.Interpret(context, setup);
        }

        [Test]
        public void Evaluate_ShouldThrowVariableNotDefinedException_GivenNullSymbolTableAndVariableExpression()
        {
            var expression = new VariableExpression("BAB");
            var context = new DiceNotationContext(expression);

            Assert.That(() => _interpreter.Interpret(context), Throws.TypeOf<SymbolTableUndefinedException>());
        }

        public class ResultsTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData(
                    (Func<IDiceRoller, IExpression>)
                    (roller =>
                        new DropExpression(
                            new DiceExpression(roller, Dice.d20, 5))))
                    .Returns(40)
                    .SetName("{m} Drop 3 lowest");
                yield return new TestCaseData(
                    (Func<IDiceRoller, IExpression>)
                    (roller =>
                    new DropExpression(
                        new DiceExpression(roller, Dice.d20, 3), x => new int[] { x.Max() })))
                    .Returns(9)
                    .SetName("{m} Drop 3 highest");
                yield return new TestCaseData(
                    (Func<IDiceRoller, IExpression>)
                    (roller =>
                    new KeepExpression(
                        new DiceExpression(roller, Dice.d20, 4), 3)))
                    .Returns(35)
                    .SetName("{m} Keep 3 highest");
            }
        }

        [TestCaseSource(typeof(ResultsTestCaseData))]
        public double Evaluate_ReturnsCorrectResult(Func<IDiceRoller, IExpression> lazyExpression)
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollDice(It.IsAny<Dice>()))
                .Returns(6)
                .Returns(3)
                .Returns(17)
                .Returns(12)
                .Returns(5);

            return _interpreter.Interpret(lazyExpression(mockRoller.Object));
        }

        [Test]
        public void Visit_ShouldSetValue_WhenVisitingNumberExpression()
        {
            var fivePointThree = new NumberExpression(5.3f);

            var result = _interpreter.Interpret(fivePointThree);

            Assert.That(result, Is.EqualTo(5.3f));
        }

        [Test]
        public void Visit_ShouldNegateNumber_GivenNegateExpression()
        {
            var five = new NumberExpression(5);
            var negateFive = new NegateExpression(five);

            var result = _interpreter.Interpret(negateFive);

            Assert.That(result, Is.EqualTo(-5));
        }

        [Test]
        public void Visit_ShouldReturnSameNumber_GivenDoubleNegative()
        {
            var number = new NumberExpression(3.7f);
            var negativeNumber = new NegateExpression(new NegateExpression(number));

            var result = _interpreter.Interpret(negativeNumber);

            Assert.That(result, Is.EqualTo(3.7f));
        }

        [Test]
        public void Visit_ShouldReturnRolledNumber_GivenDiceExpression()
        {
            var expression = new DiceExpression(_mockRoller.Object, new Dice(6), 3);

            var result = _interpreter.Interpret(expression);

            Assert.That(result, Is.EqualTo(8f));
        }

        [Test]
        public void Visit_ShouldReturnSumOfExpression_GivenRepeatExpression()
        {
            var dice = new DiceExpression(_mockRoller.Object, new Dice(6), 2);
            var critical = new RepeatExpression(dice, 2);

            var result = _interpreter.Interpret(critical);

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNull()
        {
            Assert.That(() => _interpreter.Interpret((RepeatExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldAddExpressions_GivenAdditionExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Interpret(new AdditionExpression(three, five));

            Assert.That(result, Is.EqualTo(8f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullAdditionExpression()
        {
            Assert.That(() => _interpreter.Interpret((AdditionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullSubtractionExpression()
        {
            Assert.That(() => _interpreter.Interpret((SubtractionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldSubtractExpressions_GivenSubtractionExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Interpret(new SubtractionExpression(three, five));

            Assert.That(result, Is.EqualTo(-2f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullMultiplicationExpression()
        {
            Assert.That(() => _interpreter.Interpret((MultiplicationExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldMultiplyExpressions_GivenMultiplicationExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Interpret(new MultiplicationExpression(three, five));

            Assert.That(result, Is.EqualTo(15f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullDivitionExpression()
        {
            Assert.That(() => _interpreter.Interpret((DivisionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldDivideExpressions_GivenDivitionExpression()
        {
            var three = new NumberExpression(5);
            var five = new NumberExpression(2);

            var result = _interpreter.Interpret(new DivisionExpression(three, five));

            Assert.That(result, Is.EqualTo(2.5f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullListExpression()
        {
            Assert.That(() => _interpreter.Interpret((ListExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldReturnSumOfList_GivenListExpression()
        {
            var numbers = new List<IExpression>()
            {
                new NumberExpression(3.5),
                new NumberExpression(2),
                new NumberExpression(7)
            };
            var expression = new ListExpression(numbers);

            var result = _interpreter.Interpret(expression);

            Assert.That(result, Is.EqualTo(12.5));
        }

        [Test]
        public void Interpret_ShouldThrowArgumentNullException_GivenNullContext()
        {
            Assert.That(() => _interpreter.Interpret((DiceNotationContext)null), Throws.ArgumentNullException);
        }

        [Test]
        public void DiceNotationContext_ShouldThrowArgumentNullException_GivenNullExpression()
        {
            Assert.That(() => new DiceNotationContext(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Interpret_ShouldReturnCalculatedResult_GivenExpression()
        {
            // 5 * 2 + 3.2
            var multiplication = new MultiplicationExpression(new NumberExpression(5), new NumberExpression(2));
            var addition = new AdditionExpression(multiplication, new NumberExpression(3.2));

            var result = _interpreter.Interpret(new DiceNotationContext(addition));

            Assert.That(result, Is.EqualTo(13.2));
        }
    }
}
