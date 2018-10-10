using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMTools.Dice;

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
        public void RollSeveralDiceString()
        {
            DiceRoller diceCup = new DiceRoller("5d12", new MockRandomGenerator());

            CollectionAssert.AreEqual(new List<int> { 5, 5, 5, 5, 5 }, diceCup.Roll().IndividualRolls);
        }

        [TestMethod]
        public void RollSeveralDice()
        {
            Dice d12 = new Dice(12, new MockRandomGenerator());
            DiceRoller diceCup = new DiceRoller(5, d12);

            CollectionAssert.AreEqual(new List<int> { 5, 5, 5, 5, 5 }, diceCup.Roll().IndividualRolls);
        }

        [TestMethod]
        public void ResultImmutable()
        {
            List<int> rollList = new List<int>() { 5, 2, 7, 12 };
            DiceResult diceResult = new DiceResult(rollList.Sum(), rollList);

            rollList[2] = 10;

            List<int> expected = new List<int>() { 5, 2, 7, 12 };

            CollectionAssert.AreEqual(expected, diceResult.IndividualRolls);
        }
    }
}
