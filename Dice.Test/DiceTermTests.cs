using System;
using System.Collections.Generic;
using System.Linq;
using DMTools.Die;
using DMTools.Die.Term;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiceTest
{
    [TestClass]
    public class DiceTermTests
    {
        [TestMethod]
        public void CoreDice()
        {
            Dice dice = new Dice(6, new MockRandomGenerator());

            Assert.AreEqual(5, dice.GetResults().First());
        }

        [TestMethod]
        public void DiceTimesFive()
        {
            Dice dice = new Dice(6, new MockRandomGenerator());
            TimesTerm timesFive = new TimesTerm(dice, 5);

            List<int> expected = new List<int>() { 5, 5, 5, 5, 5 };
            List<int> actual = timesFive.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTimesFiveGivenNullDice()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TimesTerm(null, 1));
        }

        [TestMethod]
        public void DiceTimesGivenZeroTimes()
        {
            Dice dice = new Dice(6, new MockRandomGenerator());
            TimesTerm timesZero = new TimesTerm(dice, 0);

            List<int> expected = new List<int>() { 5 };
            List<int> actual = timesZero.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTimesGivenNegative()
        {
            Dice dice = new Dice(6, new MockRandomGenerator());
            TimesTerm timesZero = new TimesTerm(dice, -25);

            List<int> expected = new List<int>() { 5 };
            List<int> actual = timesZero.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceDropLowestTwo()
        {
            Dice dice = new Dice(6, new MockRandomGenerator());
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestTerm dropLowest = new DropLowestTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 5, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceDropLowestPreserveOrdering()
        {
            Dice dice = new Dice(6, new MockListGenerator(new int[] { 2, 5, 3, 4, 5 }));
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestPreserveOrderingTerm dropLowest = new DropLowestPreserveOrderingTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 4, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DropLowestPreserveOrderingMinAtEnd()
        {
            Dice dice = new Dice(6, new MockListGenerator(new int[] { 2, 5, 4, 5, 1 }));
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestPreserveOrderingTerm dropLowest = new DropLowestPreserveOrderingTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 4, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTakeHighestThreePreserveOrdering()
        {
            Dice dice = new Dice(6, new MockListGenerator(new int[] { 2, 5, 3, 4, 6 }));
            TimesTerm timesFive = new TimesTerm(dice, 5);
            TakeHighestPreserveOrderingTerm takeHighest = new TakeHighestPreserveOrderingTerm(timesFive, 3);

            List<int> expected = new List<int>() { 5, 4, 6 };
            List<int> actual = takeHighest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTakeHighestThree()
        {

            Dice dice = new Dice(6, new MockListGenerator(new int[] { 2, 5, 3, 4, 6 }));
            TimesTerm timesFive = new TimesTerm(dice, 5);
            TakeHighestTerm takeHighest = new TakeHighestTerm(timesFive, 3);

            List<int> expected = new List<int>() { 6, 5, 4 };
            List<int> actual = takeHighest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
