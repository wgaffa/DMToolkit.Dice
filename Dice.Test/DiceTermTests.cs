using System;
using System.Collections.Generic;
using System.Linq;
using DMTools.Die;
using DMTools.Die.Rollers;
using DMTools.Die.Term;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiceTest
{
    [TestClass]
    public class DiceTermTests
    {
        [TestMethod]
        public void CoreDice()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);

            Assert.AreEqual(5, dice.GetResults().First());
        }

        [TestMethod]
        public void DiceTimesFive()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
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
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesZero = new TimesTerm(dice, 0);

            List<int> expected = new List<int>() { 5 };
            List<int> actual = timesZero.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTimesGivenNegative()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesZero = new TimesTerm(dice, -25);

            List<int> expected = new List<int>() { 5 };
            List<int> actual = timesZero.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceDropLowestTwo()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestTerm dropLowest = new DropLowestTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 5, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceDropLowestPreserveOrdering()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollDice(It.IsAny<PositiveInteger>()))
                .Returns(2)
                .Returns(5)
                .Returns(3)
                .Returns(4)
                .Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestPreserveOrderingTerm dropLowest = new DropLowestPreserveOrderingTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 4, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DropLowestPreserveOrderingMinAtEnd()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollDice(It.IsAny<PositiveInteger>()))
                .Returns(2)
                .Returns(5)
                .Returns(4)
                .Returns(5)
                .Returns(1);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestPreserveOrderingTerm dropLowest = new DropLowestPreserveOrderingTerm(timesFive, 2);

            List<int> expected = new List<int>() { 5, 4, 5 };
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTakeHighestThreePreserveOrdering()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollDice(It.IsAny<PositiveInteger>()))
                .Returns(2)
                .Returns(5)
                .Returns(3)
                .Returns(4)
                .Returns(6);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            TakeHighestPreserveOrderingTerm takeHighest = new TakeHighestPreserveOrderingTerm(timesFive, 3);

            List<int> expected = new List<int>() { 5, 4, 6 };
            List<int> actual = takeHighest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DiceTakeHighestThree()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollDice(It.IsAny<PositiveInteger>()))
                .Returns(2)
                .Returns(5)
                .Returns(3)
                .Returns(4)
                .Returns(6);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            TakeHighestTerm takeHighest = new TakeHighestTerm(timesFive, 3);

            List<int> expected = new List<int>() { 6, 5, 4 };
            List<int> actual = takeHighest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DropLowestPreservingOrderWhenDroppingAlot()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            DropLowestPreserveOrderingTerm dropLowest = new DropLowestPreserveOrderingTerm(timesFive, 1000);

            List<int> expected = new List<int>();
            List<int> actual = dropLowest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TakeHighestPreservingOrderWhenTakingAlot()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<PositiveInteger>())).Returns(5);
            Dice dice = new Dice(6, mockRoller.Object);
            TimesTerm timesFive = new TimesTerm(dice, 5);
            TakeHighestPreserveOrderingTerm takeHighest = new TakeHighestPreserveOrderingTerm(timesFive, 1000);

            List<int> expected = new List<int>() { 5, 5, 5, 5, 5 };
            List<int> actual = takeHighest.GetResults().ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
