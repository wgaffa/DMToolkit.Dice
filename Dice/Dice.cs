using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DMTools.Die.Rollers;

namespace Wgaffa.DMToolkit
{
    /// <summary>
    /// Represent a physical die with a set number of sides.
    /// </summary>
    public class Dice : ValueObject<Dice>
    {
        /// <summary>
        /// Constructor to create a die with DefaultRandomGenerator for randomizing
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        public Dice(int sides)
        {
            Guard.Against.NegativeOrZero(sides, nameof(sides));

            Sides = (int)sides;
        }

        /// <summary>
        /// Number of sides of the die
        /// </summary>
        public int Sides { get; }

        public override bool Equals(Dice other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Sides == other.Sides;
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return Sides;
        }
    }
}
