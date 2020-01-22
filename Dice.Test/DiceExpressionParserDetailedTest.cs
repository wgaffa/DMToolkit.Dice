using System;
using DMTools.Die.Algorithm;
using DMTools.Die.Parser;
using DMTools.Die.Rollers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiceTest
{
    [TestClass]
    public class DiceExpressionParserDetailedTest
    {
        [TestMethod]
        public void ConstantTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(3);
            string input = "5.3";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(5.3, expr.Calculate());
        }

        [TestMethod]
        public void AdditionConstantTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5.3 + 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(7.3, expr.Calculate());
        }

        [TestMethod]
        public void SubtractConstantTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5.3 - 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(3.3, expr.Calculate());
        }

        [TestMethod]
        public void MultiplyConstantTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5.3 * 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(10.6, expr.Calculate());
        }

        [TestMethod]
        public void DivisionConstantTest()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5.4 / 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(2.7, expr.Calculate());
        }

        [TestMethod]
        public void AdditionNegated()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5.3 + -2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(3.3, expr.Calculate());
        }

        [TestMethod]
        public void ComplexAlgorithm()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5 * 3 + 12 / 4 - 1";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(17, expr.Calculate());
        }

        [TestMethod]
        public void OneDice()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(1);
            string input = "d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(1, expr.Calculate());
        }

        [TestMethod]
        public void SeveralDice()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(2);
            string input = "5d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(10, expr.Calculate());
        }

        [TestMethod]
        public void DiceExpression()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(3);
            string input = "3d6 + 1d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(12, expr.Calculate());
        }

        [TestMethod]
        public void DiceOutputString()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(3);
            string input = "3d6 + 1d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual("[3, 3, 3] + [3]", expr.ToString());
        }

        [TestMethod]
        public void ComplexAlgorithmToString()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(5);
            string input = "5 * 3 + 12 / 4 - 1";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual("5 * 3 + 12 / 4 - 1", expr.ToString());
        }

        [TestMethod]
        public void D100WithPercentCharacter()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(75);
            string input = "d%";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(75, expr.Calculate());
        }

        [TestMethod]
        public void D100FiveTimesWithPercentCharacter()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollDice(It.IsAny<int>())).Returns(10);
            string input = "5d%";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(mockRoller.Object);

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(50, expr.Calculate());
        }
    }
}
