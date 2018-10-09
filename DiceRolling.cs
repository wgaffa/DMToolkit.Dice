using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMTools.Dice;

namespace DiceTest
{
    class MockRandomGenerator : IRandomGenerator
    {
        public int Generate(int min, int max)
        {
            return 5;
        }
    }

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
        public void RollSeveralDiceString()
        {
            DiceRoller diceCup = new DiceRoller("5d12", new MockRandomGenerator());

            CollectionAssert.AreEqual(new List<int> { 5, 5, 5, 5, 5 }, diceCup.Roll().ToList());
        }

        [TestMethod]
        public void RollSeveralDice()
        {
            Dice d12 = new Dice(12, new MockRandomGenerator());
            DiceRoller diceCup = new DiceRoller(5, d12);

            CollectionAssert.AreEqual(new List<int> { 5, 5, 5, 5, 5 }, diceCup.Roll().ToList());
        }
    }
}
