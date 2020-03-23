using Ardalis.GuardClauses;
using DMTools.Die.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.DiceRollers;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DiceExpression : IExpression
    {
        public Dice Dice { get; }
        public int NumberOfRolls { get; }
        public IDiceRoller DiceRoller { get; } = new StandardDiceRoller();

        public DiceExpression(Dice dice, int numberOfRolls = 1)
        {
            Guard.Against.NegativeOrZero(numberOfRolls, nameof(numberOfRolls));
            Guard.Against.Null(dice, nameof(dice));

            Dice = dice;
            NumberOfRolls = numberOfRolls;
        }

        public DiceExpression(IDiceRoller diceRoller, Dice dice, int numberOfRolls = 1)
            : this(dice, numberOfRolls)
        {
            Guard.Against.Null(diceRoller, nameof(diceRoller));

            DiceRoller = diceRoller;
        }

        public override string ToString()
        {
            return $"<dice: rolls={NumberOfRolls}, d{Dice.Sides}>";
        }
    }
}
