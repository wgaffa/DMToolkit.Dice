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
            return _diceRoller.Roll().Result;
        }

        private readonly DiceRoller _diceRoller;
    }
}
