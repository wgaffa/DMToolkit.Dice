using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DiceExpression : IExpression
    {
        public Dice Dice { get; }
        public int NumberOfRolls { get; }

        public DiceExpression(Dice dice, int numberOfRolls = 1)
        {
            Guard.Against.NegativeOrZero(numberOfRolls, nameof(numberOfRolls));
            Guard.Against.Null(dice, nameof(dice));

            Dice = dice;
            NumberOfRolls = numberOfRolls;
        }
    }
}
