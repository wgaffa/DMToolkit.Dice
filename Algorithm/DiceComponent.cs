using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class DiceComponent : IComponent
    {
        public DiceComponent(DiceRoller diceExpression)
        {
            _rollResult = diceExpression.Roll();
        }

        public double Calculate()
        {
            return _rollResult.Result;
        }

        public override string ToString()
        {
            return "[" + String.Join(", ", _rollResult.IndividualRolls) + "]";
        }

        private DiceResult _rollResult;
    }
}
