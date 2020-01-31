using Ardalis.GuardClauses;
using DMTools.Die.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationInterpreter
    {
        private readonly IDiceRoller _diceRoller;

        public DiceNotationInterpreter()
        {
            _diceRoller = new StandardDiceRoller();
        }

        public DiceNotationInterpreter(IDiceRoller diceRoller)
        {
            Guard.Against.Null(diceRoller, nameof(diceRoller));

            _diceRoller = diceRoller;
        }

        #region Terminal expressions
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by other members in class")]
        public float Visit(NumberExpression number)
        {
            Guard.Against.Null(number, nameof(number));

            return number.Value;
        }

        public float Visit(NumberListExpression list)
        {
            Guard.Against.Null(list, nameof(list));

            return list.Values.Sum();
        }

        public float Visit(DiceExpression dice)
        {
            Guard.Against.Null(dice, nameof(dice));

            return Enumerable
                .Range(0, dice.NumberOfRolls)
                .Aggregate(0, (acc, _) => acc + _diceRoller.RollDice(dice.Dice));
        }
        #endregion

        public float Visit(NegateExpression negate)
        {
            Guard.Against.Null(negate, nameof(negate));

            return -Visit((dynamic)negate.Right);
        }

        public float Visit(RepeatExpression repeat)
        {
            Guard.Against.Null(repeat, nameof(repeat));

            return Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(0f, (acc, _) => acc + Visit((dynamic)repeat.Right));
        }

        #region Binary Expressions
        public float Visit(AdditionExpression addition)
        {
            Guard.Against.Null(addition, nameof(addition));

            return Visit((dynamic)addition.Left) + Visit((dynamic)addition.Right);
        }

        public float Visit(SubtractionExpression subtraction)
        {
            Guard.Against.Null(subtraction, nameof(subtraction));

            return Visit((dynamic)subtraction.Left) - Visit((dynamic)subtraction.Right);
        }

        public float Visit(MultiplicationExpression multiplication)
        {
            Guard.Against.Null(multiplication, nameof(multiplication));

            return Visit((dynamic)multiplication.Left) * Visit((dynamic)multiplication.Right);
        }

        public float Visit(DivisionExpression divition)
        {
            Guard.Against.Null(divition, nameof(divition));

            return Visit((dynamic)divition.Left) / Visit((dynamic)divition.Right);
        }
        #endregion
    }
}
