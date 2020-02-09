using Wgaffa.DMToolkit;

namespace Wgaffa.DMToolkit.DiceRollers
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
        int RollDice(Dice dice);
    }
}
