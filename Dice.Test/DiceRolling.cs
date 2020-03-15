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
    }
}
