using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DMTools.Die;
using DMTools.Die.Parser;
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
    }
}
