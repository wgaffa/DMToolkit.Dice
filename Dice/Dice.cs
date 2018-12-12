using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DMTools.Die.Rollers;
using DMTools.Die.Term;

namespace DMTools.Die
{
    /// <summary>
    /// Represent a physical die with a set number of sides.
    /// </summary>
    public class Dice : IDiceTerm
    {
        /// <summary>
        /// Constructor to create a die with DefaultRandomGenerator for randomizing
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        public Dice(int sides = 6)
        {
            Sides = sides < 1 ? throw new ArgumentException("Dice sides must be a positive number") : sides;
        }

        /// <summary>
        /// Constructor to create a die
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        /// <param name="diceRoller">Random generator to generate random numbers from</param>
        public Dice(int sides, IDiceRoller diceRoller)
        {
            Sides = sides < 1 ? throw new ArgumentException("Dice sides must be a positive number") : sides;
            _diceRoller = diceRoller ?? throw new ArgumentNullException(nameof(diceRoller));
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
        public int Sides { get; private set; }

        /// <summary>
        /// Random number generator when rolling die
        /// </summary>
        private readonly IDiceRoller _diceRoller = new StandardDiceRoller();
    }
}
