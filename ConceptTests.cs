using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DMTools.Dice;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DiceTest
{
    [TestClass]
    public class ConceptTests
    {
        enum DiceToken
        {
            Unidentified,
            Dice,
            Number,
            Plus,
            Minus
        }

        [TestMethod]
        public void SimpleParserConcept()
        {
            var parseA = Character.EqualTo('A').AtLeastOnce();

            var result = parseA.Parse("AAAA");

            Assert.AreEqual("AAAA", new string(result));
        }

        [TestMethod]
        public void SimpleParserDiceConcept()
        {
            TextParser<DiceRoller> diceParser =
                from rolls in Numerics.Natural.OptionalOrDefault(new TextSpan("1"))
                from _ in Character.In(new char[] { 'd', 'D' })
                from sides in Numerics.Natural
                select new DiceRoller(int.Parse(rolls.ToString()), new Dice(int.Parse(sides.ToString()), new MockRandomGenerator()));

            var dice = diceParser.Parse("d10");

            CollectionAssert.AreEqual(new List<int> { 5 }, dice.Roll().IndividualRolls);
        }

        [TestMethod]
        public void SimpleTokenizerConcept()
        {
            var diceParser =
                from rolls in Numerics.Natural.Optional()
                from _ in Character.In(new char[] { 'd', 'D' })
                from sides in Numerics.Natural
                select rolls.ToString() + 'd' + sides.ToString();

            var tokenizer = new TokenizerBuilder<DiceToken>()
                .Ignore(Span.WhiteSpace)
                .Match(diceParser, DiceToken.Dice)
                .Match(Character.EqualTo('+'), DiceToken.Plus)
                .Match(Character.EqualTo('-'), DiceToken.Minus)
                .Match(Numerics.Natural, DiceToken.Number)
                .Build();

            var tokenList = tokenizer.Tokenize("1d6+5");

            Assert.Fail();
        }

        [TestMethod]
        public void ExpressionSimpleConcept()
        {
            Expression constant5 = Expression.Constant(5);
            Expression constant7 = Expression.Constant(7);
            Expression addition = Expression.Add(constant5, constant7);
            LambdaExpression lambda = Expression.Lambda(addition);

            var result = (Func<int>)lambda.Compile();

            Assert.AreEqual(12, result());
        }

        [TestMethod]
        public void ExpressionCallObjectConcept()
        {
            Dice sixSidedDice = new Dice(6, new MockRandomGenerator());

            //Expression<Func<int>> expr = () => sixSidedDice.Roll();

            var method = typeof(Dice).GetMethod("Roll", BindingFlags.Instance | BindingFlags.Public);
            Expression instance = Expression.Constant(sixSidedDice);
            Expression callRoll = Expression.Call(instance, method);
            LambdaExpression expr = Expression.Lambda(callRoll);

            var expected = (Func<int>)expr.Compile();
            Assert.AreEqual(5, expected());
        }

        [TestMethod]
        public void ExpressionRollWithArithmeticConcept()
        {
            Dice twelveSidedDice = new Dice(12, new MockRandomGenerator());
            DiceRoller diceCup = new DiceRoller(3, twelveSidedDice);

            var rollMethod = typeof(DiceRoller).GetMethod("Roll");
            Expression instance = Expression.Constant(diceCup);
            Expression rollCall = Expression.Call(instance, rollMethod);
            Expression diceResult = Expression.Property(rollCall, "Result");
            ParameterExpression left = Expression.Variable(typeof(int), "left");
            Expression assignRoll = Expression.Assign(left, diceResult);

            Expression constant7 = Expression.Constant(7);
            Expression addition = Expression.MakeBinary(ExpressionType.Add, assignRoll, constant7);
            Expression block = Expression.Block(new[] { left }, addition);
            LambdaExpression lambda = Expression.Lambda(block);

            var expected = (Func<int>)lambda.Compile();

            Assert.AreEqual(22, expected());
        }
    }
}
