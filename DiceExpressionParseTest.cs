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
        public void ParseConstant()
        {
            string input = "223";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(223, expr.Compile()());
        }
        [TestMethod]
        public void ParseSingleDiceExpression()
        {
            DiceExpressionParser.RandomGenerator = new MockRandomGenerator(57);

            string input = "d100";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(57, expr.Compile()());
        }

        [TestMethod]
        public void ParseWrongDiceInput()
        {
            string input = "d 100";

            Assert.ThrowsException<ParseException>(() => new DiceExpressionTokenizer().Tokenize(input));
        }

        [TestMethod]
        public void ParseSimpleDiceExpression()
        {
            string input = "5d20";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(11, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticDiceExpression()
        {
            string input = "1d6 + 1d4";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(6, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticDiceConstantExpression()
        {
            string input = "1d8 * 1.5";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(9, expr.Compile()());
        }

        [TestMethod]
        public void ParseArithmeticIntegerExpression()
        {
            string input = "1d8 * 3";

            var tokenizer = new DiceExpressionTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var expr = DiceExpressionParser.Lambda.Parse(tokens);

            Assert.AreEqual(18, expr.Compile()());

        }

        [TestMethod]
        public void SimpleArithmeticExpression()
        {
            var tokenParser = new DiceExpressionTokenizer();
            var tok = tokenParser.Tokenize("16 - 5*2");

            var result = DiceExpressionParser.Lambda.Parse(tok);
            var expr = result.Compile();

            Console.WriteLine(expr());

            Assert.AreEqual(6, expr());
        }
    }
}
