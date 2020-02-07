// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMTools.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void Ctor_NumberExpression_ShouldSetValue()
        {
            var number = new NumberExpression(3.5f);

            Assert.That(number.Value, Is.EqualTo(3.5));
        }

        [Test]
        public void NegateOperation_ShouldSetRightSide()
        {
            var five = new NumberExpression(5);
            var negativeFive = new NegateExpression(five);

            Assert.That(negativeFive.Right, Is.InstanceOf<NumberExpression>());
        }

        [Test]
        public void NegateExpression_ShoulThrowArgumentNullException_GivenNull()
        {
            Assert.That(() => new NegateExpression(null), Throws.ArgumentNullException);
        }

        [TestCase(0)]
        [TestCase(-6)]
        public void DiceExpression_ShouldThrowArgumentException_GivenNonPositiveRolls(int rolls)
        {
            Assert.That(() => new DiceExpression(new Dice(6), rolls), Throws.ArgumentException);
        }

        [Test]
        public void DiceExpression_ShouldSetNumberOfRolls()
        {
            var dice = new Dice(6);
            var diceExpression = new DiceExpression(dice, 4);

            Assert.That(diceExpression.NumberOfRolls, Is.EqualTo(4));
        }

        [Test]
        public void DiceExpression_ShouldSetDice()
        {
            var diceExpression = new DiceExpression(new Dice(6));

            Assert.That(diceExpression.Dice, Is.EqualTo(new Dice(6)));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void RepeatExpression_ShouldThrowArgumentException_GivenInvalidRange(int repeat)
        {
            Assert.That(() => new RepeatExpression(new NumberExpression(5), repeat), Throws.ArgumentException);
        }

        [Test]
        public void RepeatExpression_ShouldThrowArgumentNullException_GivenNullExpression()
        {
            Assert.That(() => new RepeatExpression(null, 3), Throws.ArgumentNullException);
        }

        [Test]
        public void RepeatExpression_ShouldSetRepeatTimes()
        {
            var repeat = new RepeatExpression(new NumberExpression(5), 3);

            Assert.That(repeat.RepeatTimes, Is.EqualTo(3));
        }

        [Test]
        public void AdditionExpression_ShouldSetLeft()
        {
            var left = new NumberExpression(3);
            var right = new NumberExpression(5);

            var addition = new AdditionExpression(left, right);

            var expected = new NumberExpression(3);
            Assert.That(addition.Left, Is.EqualTo(expected));
        }

        [Test]
        public void AdditionExpression_ShouldThrowArgumentNullException_GivenNullLeft()
        {
            Assert.That(() => new AdditionExpression(null, new NumberExpression(5)), Throws.ArgumentNullException);
        }

        [Test]
        public void AdditionExpression_ShouldThrowArgumentNullException_GivenNullRight()
        {
            Assert.That(() => new AdditionExpression(new NumberExpression(3), null), Throws.ArgumentNullException);
        }

        [Test]
        public void NumberListExpression_ShouldThrowArgumentNullException_GivenNullSequence()
        {
            Assert.That(() => new ListExpression(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ListExpression_ShouldSetSequence()
        {
            var numbers = new List<IExpression>
            {
                new NumberExpression(2),
                new NumberExpression(3.5f),
                new NumberExpression(3)
            };
            var listExpression = new ListExpression(numbers);

            var expected = new List<IExpression>
            {
                new NumberExpression(2),
                new NumberExpression(3.5f),
                new NumberExpression(3)
            };

            Assert.That(listExpression.Expressions, Is.EquivalentTo(expected));
        }
    }
}
