using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMTools.Die;
using Moq;

namespace DiceTest
{
    [TestClass]
    public class DiceRolling
    {
        [TestMethod]
        public void RollDice()
        {
            Dice d12 = new Dice(12, new MockRandomGenerator());

            Assert.AreEqual(5, d12.Roll());
        }

        [TestMethod]
        public void CreateDiceNullRandomGenerator()
        {
            Dice standardDice = new Dice(6, null);

            Assert.IsTrue(standardDice.Roll() < 7);
        }

        [TestMethod]
        public void CreateNegativeDiceSides()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dice(-4, new MockRandomGenerator()));
        }

        [TestMethod]
        public void CreateZeroSidedDice()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dice(0, new MockRandomGenerator()));
        }
    }
}
