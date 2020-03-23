using System;
using Moq;
using DMTools.Die.Rollers;
using Wgaffa.DMToolkit;
using NUnit.Framework;

namespace DiceTest
{
    public class DiceRolling
    {
        [Test]
        public void CreateNegativeDiceSides()
        {
            Assert.That(() => new Dice(-4), Throws.ArgumentException);
        }

        [Test]
        public void CreateZeroSidedDice()
        {
            Assert.That(() => new Dice(0), Throws.ArgumentException);
        }

        [Test]
        public void Equal_ShouldReturnTrue_GivenSameReference()
        {
            var dice = new Dice(66);
            var refDice = dice;

            Assert.That(dice, Is.EqualTo(refDice));
        }

        [Test]
        public void GetHashCode_ShouldReturnHashCode()
        {
            var expected = 17 * 213 + 6;

            Assert.That(Dice.d6.GetHashCode(), Is.EqualTo(expected));
        }
    }
}
