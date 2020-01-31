using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using DMTools.Die.Rollers;
using Wgaffa.DMToolkit;

namespace DiceTest
{
    [TestClass]
    public class DiceRolling
    {
        [TestMethod]
        public void CreateNegativeDiceSides()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dice(-4));
        }

        [TestMethod]
        public void CreateZeroSidedDice()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dice(0));
        }
    }
}
