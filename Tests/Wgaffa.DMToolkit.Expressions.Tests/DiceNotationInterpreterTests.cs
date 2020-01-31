using DMTools.Die.Rollers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit;
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

            var result = _interpreter.Visit(fivePointThree);

            Assert.That(result, Is.EqualTo(5.3f));
        }

        [Test]
        public void Visit_ShouldNegateNumber_GivenNegateExpression()
        {
            var five = new NumberExpression(5);
            var negateFive = new NegateExpression(five);

            var result = _interpreter.Visit(negateFive);

            Assert.That(result, Is.EqualTo(-5));
        }

        [Test]
        public void Visit_ShouldReturnSameNumber_GivenDoubleNegative()
        {
            var number = new NumberExpression(3.7f);
            var negativeNumber = new NegateExpression(new NegateExpression(number));

            var result = _interpreter.Visit(negativeNumber);

            Assert.That(result, Is.EqualTo(3.7f));
        }

        [Test]
        public void Visit_ShouldReturnRolledNumber_GivenDiceExpression()
        {
            var expression = new DiceExpression(new Dice(6), 3);

            var result = _interpreter.Visit(expression);

            Assert.That(result, Is.EqualTo(8f));
        }

        [Test]
        public void Visit_ShouldReturnSumOfExpression_GivenRepeatExpression()
        {
            var dice = new DiceExpression(new Dice(6), 2);
            var critical = new RepeatExpression(dice, 2);

            var result = _interpreter.Visit(critical);

            Assert.That(result, Is.EqualTo(14f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNull()
        {
            Assert.That(() => _interpreter.Visit((RepeatExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldAddExpressions_GivenAdditionExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Visit(new AdditionExpression(three, five));

            Assert.That(result, Is.EqualTo(8f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullAdditionExpression()
        {
            Assert.That(() => _interpreter.Visit((AdditionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullSubtractionExpression()
        {
            Assert.That(() => _interpreter.Visit((SubtractionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldSubtractExpressions_GivenSubtractionExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Visit(new SubtractionExpression(three, five));

            Assert.That(result, Is.EqualTo(-2f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullMultiplicationExpression()
        {
            Assert.That(() => _interpreter.Visit((MultiplicationExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldMultiplyExpressions_GivenMultiplicationExpression()
        {
            var three = new NumberExpression(3);
            var five = new NumberExpression(5);

            var result = _interpreter.Visit(new MultiplicationExpression(three, five));

            Assert.That(result, Is.EqualTo(15f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullDivitionExpression()
        {
            Assert.That(() => _interpreter.Visit((DivisionExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldDivideExpressions_GivenDivitionExpression()
        {
            var three = new NumberExpression(5);
            var five = new NumberExpression(2);

            var result = _interpreter.Visit(new DivisionExpression(three, five));

            Assert.That(result, Is.EqualTo(2.5f));
        }

        [Test]
        public void Visit_ShouldThrowArgumentNullException_GivenNullNumberListExpression()
        {
            Assert.That(() => _interpreter.Visit((NumberListExpression)null), Throws.ArgumentNullException);
        }

        [Test]
        public void Visit_ShouldReturnSumOfList_GivenNumberListExpression()
        {
            var expression = new NumberListExpression(new float[] { 3.5f, 2, 7 });

            var result = _interpreter.Visit(expression);

            Assert.That(result, Is.EqualTo(12.5f));
        }
    }
}
