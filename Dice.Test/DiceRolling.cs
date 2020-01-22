using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMTools.Die;
using Moq;
using DMTools.Die.Rollers;

namespace DiceTest
{
    [TestClass]
    public class DiceRolling
    {
        [TestMethod]
        public void RollDice()
        {
            var mockRandom = new Mock<IDiceRoller>();
            mockRandom.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            Dice d12 = new Dice(12, mockRandom.Object);

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
            Assert.ThrowsException<ArgumentException>(() => new Dice(-4, null));
        }

        [TestMethod]
        public void CreateZeroSidedDice()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dice(0, null));
        }
    }
}
