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
    }
}
