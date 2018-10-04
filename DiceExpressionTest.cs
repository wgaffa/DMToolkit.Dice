using DMTools.Dice;
using DMTools.Dice.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceTest
{
    [TestClass]
    public class DiceExpressionTest
    {
        class MockRandomGenerator : IRandomGenerator
        {
            private readonly int _constant;

            public MockRandomGenerator(int constant = 5)
            {
                _constant = constant;
            }

            public int Generate(int min, int max)
            {
                return _constant;
            }
        }

        [TestMethod]
        public void DiceValue()
        {
            DiceValue expression = new DiceValue(new DiceRoller("5d6", new MockRandomGenerator()));

            Assert.AreEqual(25, expression.Evaluate());
        }

        [TestMethod]
        public void ConstantMultipy()
        {
            Constant five = new Constant(5);
            Constant three = new Constant(3);
            BinaryOperation multiply = new BinaryOperation((x, y) => x * y, five, three);

            Assert.AreEqual(15, multiply.Evaluate());
        }

        [TestMethod]
        public void DiceExpression()
        {
            Constant five = new Constant(5);
            DiceValue diceExpression = new DiceValue(new DiceRoller("2d6", new MockRandomGenerator()));
            DiceValue diceOneHundred = new DiceValue(new DiceRoller("d100", new MockRandomGenerator(7)));
            BinaryOperation smallAddition = new BinaryOperation((x, y) => x + y, diceExpression, five);
            BinaryOperation oneHundredMultiply = new BinaryOperation((x, y) => x * y, smallAddition, diceOneHundred);

            Assert.AreEqual(105, oneHundredMultiply.Evaluate());
        }
    }
}
