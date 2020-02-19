using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationDisplay
    {
        private int _position = 0;

        public string Evaluate(IExpression expression)
        {
            _position = 0;
            return Visit((dynamic)expression);
        }

        #region Terminal nodes
        private string Visit(NumberExpression number)
        {
            _position++;
            return $"{number}";
        }

        private string Visit(DiceExpression dice)
        {
            _position++;
            return $"{dice}";
        }
        #endregion

        #region Unary NonTerminal
        private string Visit(NegateExpression negate)
        {
            _position++;
            switch (negate.Right)
            {
                case AdditionExpression _:
                case NumberExpression number when number.Value < 0:
                case NegateExpression _:
                    return $"-({Visit((dynamic)negate.Right)})";
                default:
                    return $"-{Visit((dynamic)negate.Right)}";
            }
        }
        #endregion

        #region Binary NonTerminal
        private string Visit(AdditionExpression addition)
        {
            var leftHandGroupPredicates = new List<Func<IExpression, bool>>()
            {
                (expr) => expr is NegateExpression && _position > 0,
                (expr) => expr is NumberExpression number && number.Value < 0 && _position > 0
            };

            Func<IExpression, string> literal = (expr) => $"{Visit((dynamic)expr)}";
            Func<IExpression, string> group = (expr) => $"({literal(expr)})";

            string leftString;
            if (leftHandGroupPredicates.Any((f) => f(addition.Left)))
                leftString = group(addition.Left);
            else
                leftString = literal(addition.Left);

            _position++;
            string rightString;
            if (leftHandGroupPredicates.Any((f) => f(addition.Right)))
                rightString = group(addition.Right);
            else
                rightString = literal(addition.Right);

            return $"{leftString}+{rightString}";
        }
        #endregion
    }
}
