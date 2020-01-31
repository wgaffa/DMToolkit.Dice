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
        /// <summary>
        /// Constructor to create a die with DefaultRandomGenerator for randomizing
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        public Dice(PositiveInteger sides)
        {
            Sides = sides;
        }

        /// <summary>
        /// Number of sides of the die
        /// </summary>
        public PositiveInteger Sides { get; }
    }
}
