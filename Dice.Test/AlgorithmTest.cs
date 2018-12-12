﻿using System;
using DMTools.Die;
using DMTools.Die.Algorithm;
using DMTools.Die.Term;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiceTest
{
    [TestClass]
    public class AlgorithmTest
    {
        [TestMethod]
        public void SimpleAlgorithm()
        {
            IComponent expr = new Adder(
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
            IComponent expr = new Multiplier(
                new DiceComponent(new TimesTerm(
                    new Dice(6, new MockRandomGenerator(3)), 3)),
                new Constant(1.5)
                );

            Assert.AreEqual(13.5, expr.Calculate());
        }

        [TestMethod]
        public void NegateTest()
        {
            IComponent expr = new Adder(
                new DiceComponent(new Dice(8, new MockRandomGenerator(4))),
                new Negate(
                    new Constant(2)
                    )
                );

            Assert.AreEqual(2, expr.Calculate());
        }
    }
}
