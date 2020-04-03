﻿// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMTools.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void Ctor_NumberExpression_ShouldSetValue()
        {
            var number = new Literal(3.5f);

            Assert.That(number.Value, Is.EqualTo(3.5));
        }

        [Test]
        public void NegateOperation_ShouldSetRightSide()
        {
            var five = new Literal(5);
            var negativeFive = new Negate(five);

            Assert.That(negativeFive.Right, Is.InstanceOf<Literal>());
        }

        [Test]
        public void NegateExpression_ShoulThrowArgumentNullException_GivenNull()
        {
            Assert.That(() => new Negate(null), Throws.ArgumentNullException);
        }

        [TestCase(0)]
        [TestCase(-6)]
        public void DiceExpression_ShouldThrowArgumentException_GivenNonPositiveRolls(int rolls)
        {
            Assert.That(() => new DiceRoll(new Dice(6), rolls), Throws.ArgumentException);
        }

        [Test]
        public void DiceExpression_ShouldSetNumberOfRolls()
        {
            var dice = new Dice(6);
            var diceExpression = new DiceRoll(dice, 4);

            Assert.That(diceExpression.NumberOfRolls, Is.EqualTo(4));
        }

        [Test]
        public void DiceExpression_ShouldSetDice()
        {
            var diceExpression = new DiceRoll(new Dice(6));

            Assert.That(diceExpression.Dice, Is.EqualTo(new Dice(6)));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void RepeatExpression_ShouldThrowArgumentException_GivenInvalidRange(int repeat)
        {
            Assert.That(() => new Repeat(new Literal(5), repeat), Throws.ArgumentException);
        }

        [Test]
        public void RepeatExpression_ShouldThrowArgumentNullException_GivenNullExpression()
        {
            Assert.That(() => new Repeat(null, 3), Throws.ArgumentNullException);
        }

        [Test]
        public void RepeatExpression_ShouldSetRepeatTimes()
        {
            var repeat = new Repeat(new Literal(5), 3);

            Assert.That(repeat.RepeatTimes, Is.EqualTo(3));
        }

        [Test]
        public void AdditionExpression_ShouldSetLeft()
        {
            var left = new Literal(3);
            var right = new Literal(5);

            var addition = new Addition(left, right);

            var expected = left;
            Assert.That(addition.Left, Is.EqualTo(expected));
        }

        [Test]
        public void AdditionExpression_ShouldThrowArgumentNullException_GivenNullLeft()
        {
            Assert.That(() => new Addition(null, new Literal(5)), Throws.ArgumentNullException);
        }

        [Test]
        public void AdditionExpression_ShouldThrowArgumentNullException_GivenNullRight()
        {
            Assert.That(() => new Addition(new Literal(3), null), Throws.ArgumentNullException);
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
                new Literal(2),
                new Literal(3.5f),
                new Literal(3)
            };
            var listExpression = new ListExpression(numbers);

            var expected = new List<IExpression>(numbers);

            Assert.That(listExpression.Expressions, Is.EquivalentTo(expected));
        }

        [Test]
        public void DiceExpression_ShouldSetDiceRoller()
        {
            var mockRoller = new Mock<IDiceRoller>();

            var expr = new DiceRoll(mockRoller.Object, new Dice(6), 2);

            Assert.That(expr.DiceRoller, Is.SameAs(mockRoller.Object));
        }

        [Test]
        public void KeepExpression_ShouldSetCount()
        {
            var keepExpression = new Keep(new DiceRoll(Dice.d20, 3), 5);

            Assert.That(keepExpression.Count, Is.EqualTo(5));
        }

        [Test]
        public void KeepExpression_ShouldThrowArgumentError_GivenNegativeCount()
        {
            Assert.That(() => new Keep(new DiceRoll(Dice.d20, 3), -3), Throws.ArgumentException);
        }
    }
}
