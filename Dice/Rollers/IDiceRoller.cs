using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Rollers
{
    /// <summary>
    /// Interface for generating a random number when rolling a dice.
    /// </summary>
    public interface IDiceRoller
    {
        /// <summary>
        /// Roll a die.
        /// </summary>
        /// <param name="sides">Number of sides on the die.</param>
        /// <returns>Random number between [1, <paramref name="sides"/>]</returns>
        int RollDice(PositiveInteger sides);
    }
}
