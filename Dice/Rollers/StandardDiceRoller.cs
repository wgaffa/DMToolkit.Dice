using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Rollers
{
    public class StandardDiceRoller : IDiceRoller
    {
        private static readonly Random Random = new Random();

        private readonly Random _random = Random;

        public StandardDiceRoller() : this(Random)
        {
        }

        public StandardDiceRoller(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public int RollDice(int sides)
        {
            return _random.Next(1, sides + 1);
        }
    }
}
