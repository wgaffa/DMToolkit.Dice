using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMTools.Die;

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
            Assert.ThrowsException<ArgumentNullException>(() => new Dice(5, null));
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
