using System;
using DMTools.Die.Algorithm;
using DMTools.Die.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiceTest
{
    [TestClass]
    public class DiceExpressionParserDetailedTest
    {
        [TestMethod]
        public void ConstantTest()
        {
            string input = "5.3";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(3));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(5.3, expr.Calculate());
        }

        [TestMethod]
        public void AdditionConstantTest()
        {
            string input = "5.3 + 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(7.3, expr.Calculate());
        }

        [TestMethod]
        public void SubtractConstantTest()
        {
            string input = "5.3 - 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(3.3, expr.Calculate());
        }

        [TestMethod]
        public void MultiplyConstantTest()
        {
            string input = "5.3 * 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(10.6, expr.Calculate());
        }

        [TestMethod]
        public void DivisionConstantTest()
        {
            string input = "5.4 / 2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(2.7, expr.Calculate());
        }

        [TestMethod]
        public void AdditionNegated()
        {
            string input = "5.3 + -2";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(3.3, expr.Calculate());
        }

        [TestMethod]
        public void ComplexAlgorithm()
        {
            string input = "5 * 3 + 12 / 4 - 1";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(17, expr.Calculate());
        }

        [TestMethod]
        public void OneDice()
        {
            string input = "d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(1));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(1, expr.Calculate());
        }

        [TestMethod]
        public void SeveralDice()
        {
            string input = "5d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(2));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(10, expr.Calculate());
        }

        [TestMethod]
        public void DiceExpression()
        {
            string input = "3d6 + 1d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(3));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(12, expr.Calculate());
        }

        [TestMethod]
        public void DiceOutputString()
        {
            string input = "3d6 + 1d4";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(3));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual("[3, 3, 3] + [3]", expr.ToString());
        }

        [TestMethod]
        public void ComplexAlgorithmToString()
        {
            string input = "5 * 3 + 12 / 4 - 1";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(5));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual("5 * 3 + 12 / 4 - 1", expr.ToString());
        }

        [TestMethod]
        public void D100WithPercentCharacter()
        {
            string input = "d%";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(75));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(75, expr.Calculate());
        }

        [TestMethod]
        public void D100FiveTimesWithPercentCharacter()
        {
            string input = "5d%";
            DiceExpressionParserDetailed parser = new DiceExpressionParserDetailed(new MockRandomGenerator(10));

            IDiceExpression expr = parser.ParseString(input);

            Assert.AreEqual(50, expr.Calculate());
        }
    }
}
