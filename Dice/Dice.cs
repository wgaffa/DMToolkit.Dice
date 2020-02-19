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
        public static readonly Dice d4 = new Dice(4);
        public static readonly Dice d6 = new Dice(6);
        public static readonly Dice d8 = new Dice(8);
        public static readonly Dice d10 = new Dice(10);
        public static readonly Dice d12 = new Dice(12);
        public static readonly Dice d20 = new Dice(20);
        public static readonly Dice d100 = new Dice(100);

        /// <summary>
        /// Constructor to create a die
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
