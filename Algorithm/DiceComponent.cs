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
            rollResult = diceExpression.Roll();
        }

        public double Calculate()
        {
            return rollResult.Result;
        }

        private DiceResult rollResult;
    }
}
