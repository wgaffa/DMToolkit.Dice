using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DMTools.Dice;
using DMTools.Dice.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DiceTest
{
    [TestClass]
    public class DiceExpressionParseTest
    {
        [TestMethod]
        public void ParseSimpleDiceExpression()
        {
            string input = "1d20";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = new DiceExpressionParser(new MockRandomGenerator(11)).Lambda.Parse(tokens);

            Assert.AreEqual(11, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticDiceExpression()
        {
            string input = "1d6 + 1d4";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = new DiceExpressionParser(new MockRandomGenerator(3)).Lambda.Parse(tokens);

            Assert.AreEqual(6, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticDiceConstantExpression()
        {
            string input = "1d8 * 1.5";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = new DiceExpressionParser(new MockRandomGenerator(6)).Lambda.Parse(tokens);

            Assert.AreEqual(9, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticIntegerExpression()
        {
            string input = "1d8 * 3";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = new DiceExpressionParser(new MockRandomGenerator(6)).Lambda.Parse(tokens);

            Assert.AreEqual(18, expr.Compile()());

        }
    }
}
