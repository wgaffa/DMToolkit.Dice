using System;
using DMTools.Die;
using DMTools.Die.Algorithm;
using DMTools.Die.Rollers;
using DMTools.Die.Term;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiceTest
{
    [TestClass]
    public class AlgorithmTest
    {
        [TestMethod]
        public void SimpleAlgorithm()
        {
            IDiceExpression expr = new Adder(
                new Constant(5.3),
                new Multiplier(
                    new Constant(3.2),
                    new Constant(7.3)
                    )
                );

            Assert.AreEqual(28.66, expr.Calculate());
        }

        [TestMethod]
        public void SimpleDiceAlgorithm()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(3);
            IDiceExpression expr = new Multiplier(
                new DiceTermExpression(new TimesTerm(
                    new Dice(6, mockRoller.Object), 3)),
                new Constant(1.5)
                );

            Assert.AreEqual(13.5, expr.Calculate());
        }

        [TestMethod]
        public void NegateTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(4);
            IDiceExpression expr = new Adder(
                new DiceTermExpression(new Dice(8, mockRoller.Object)),
                new Negate(
                    new Constant(2)
                    )
                );

            Assert.AreEqual(2, expr.Calculate());
        }
    }
}
