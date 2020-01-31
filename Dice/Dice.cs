using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DMTools.Die.Rollers;

namespace Wgaffa.DMToolkit
{
    /// <summary>
    /// Represent a physical die with a set number of sides.
    /// </summary>
    public class Dice
    {
        private static readonly IDiceRoller DefaultDiceRoller = new StandardDiceRoller();

        /// <summary>
        /// Constructor to create a die with DefaultRandomGenerator for randomizing
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        public Dice(PositiveInteger sides)
            : this(sides, null)
        {
        }

        /// <summary>
        /// Constructor to create a die
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        /// <param name="diceRoller">Random generator to generate random numbers from</param>
        public Dice(PositiveInteger sides, IDiceRoller diceRoller)
        {
            Sides = sides;
            _diceRoller = diceRoller ?? DefaultDiceRoller;
        }

        /// <summary>
        /// Rolls the die
        /// </summary>
        /// <returns>Random number between 1 and Sides</returns>
        public int Roll()
        {
            return _diceRoller.RollDice(Sides);
        }

        public IEnumerable<int> GetResults()
        {
            yield return Roll();
        }

        public Expression ToExpression()
        {
            return Expression.Constant(Roll());
        }

        /// <summary>
        /// Number of sides of the die
        /// </summary>
        public PositiveInteger Sides { get; }

        /// <summary>
        /// Random number generator when rolling die
        /// </summary>
        private readonly IDiceRoller _diceRoller = new StandardDiceRoller();
    }
}
