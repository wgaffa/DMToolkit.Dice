using DMTools.Die.Rollers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    [TestFixture]
    public class DiceNotationInterpreterTests
    {
        private DiceNotationInterpreter _interpreter;

        [SetUp]
        public void Setup()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(d => d.RollDice(It.IsAny<Dice>()))
                .Returns(2)
                .Returns(4)
                .Returns(2)
                .Returns(6)
                .Returns(1);

            _interpreter = new DiceNotationInterpreter(mockRoller.Object);
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
            var expression = new DiceExpression(new Dice(6), 3);

            var result = _interpreter.Interpret(expression);

            Assert.That(result, Is.EqualTo(8f));
        }

        [Test]
        public void Visit_ShouldReturnSumOfExpression_GivenRepeatExpression()
        {
            var dice = new DiceExpression(new Dice(6), 2);
            var critical = new RepeatExpression(dice, 2);

            var result = _interpreter.Interpret(critical);

            Assert.That(result, Is.EqualTo(14f));
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
                new NumberExpression(3.5f),
                new NumberExpression(2),
                new NumberExpression(7)
            };
            var expression = new ListExpression(numbers);

            var result = _interpreter.Interpret(expression);

            Assert.That(result, Is.EqualTo(12.5f));
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
            var addition = new AdditionExpression(multiplication, new NumberExpression(3.2f));

            var result = _interpreter.Interpret(new DiceNotationContext(addition));

            Assert.That(result, Is.EqualTo(13.2f));
        }

        [Test]
        public void Interpret_ShouldSetContextResult_GivenExpression()
        {
            var dice = new DiceExpression(new Dice(6), 3);
            var addThree = new AdditionExpression(dice, new NumberExpression(3));

            var context = new DiceNotationContext(addThree);
            _ = _interpreter.Interpret(context);

            var expected = new List<IExpression>
            {
                new NumberExpression(2),
                new NumberExpression(4),
                new NumberExpression(2)
            };
            AdditionExpression result = (AdditionExpression)context.Result;
            ListExpression list = (ListExpression)result.Left;
            Assert.That(list.Expressions, Is.EquivalentTo(expected));
        }
    }
}
