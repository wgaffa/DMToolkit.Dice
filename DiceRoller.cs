using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice
{
    /// <summary>
    /// Rolls several dices of one type, eg. 5d12 rolls a d12 5 times.
    /// </summary>
    public class DiceRoller
    {
        public DiceRoller(int rolls, Dice dice)
        {
            NumberOfRolls = rolls;
            Dice = dice;
        }

        public DiceRoller(string diceExpression, IRandomGenerator rng = null)
        {
            _randomGenerator = rng ?? new DefaultRandomGenerator();

            string[] diceSplit = diceExpression.Split(new char[] { 'd', 'D' });
            NumberOfRolls = String.IsNullOrEmpty(diceSplit[0]) ? 1 : int.Parse(diceSplit[0]);
            Dice = new Dice(int.Parse(diceSplit[1]), _randomGenerator);
        }

        public IEnumerable<int> Roll()
        {
            List<int> rolls = new List<int>();

            for (int i = 0; i < NumberOfRolls; i++)
            {
                rolls.Add(Dice.Roll());
            }

            return rolls;
        }

        public int RollSum()
        {
            return Roll().Sum();
        }

        public int NumberOfRolls { get; private set; }
        public Dice Dice { get; private set; }

        private readonly IRandomGenerator _randomGenerator = null;
    }
}
