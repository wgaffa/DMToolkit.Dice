using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Expression
{
    public class DiceValue : IDiceExpression
    {
        public DiceValue(DiceRoller roller)
        {
            _diceRoller = roller;
        }

        public int Evaluate()
        {
            List<int> rolls = _diceRoller.Roll().ToList();
            return rolls.Aggregate((lhs, rhs) => lhs + rhs);
        }

        private readonly DiceRoller _diceRoller;
    }
}
