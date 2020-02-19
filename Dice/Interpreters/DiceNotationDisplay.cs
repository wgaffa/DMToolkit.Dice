using System;
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
            string leftString;
            switch (addition.Left)
            {
                case NegateExpression _ when _position > 0:
                case NumberExpression number when number.Value < 0 && _position > 0:
                    leftString = $"({Visit((dynamic)addition.Left)})";
                    break;
                default:
                    leftString = $"{addition.Left}";
                    break;
            }

            _position++;
            switch (addition.Right)
            {
                case NumberExpression number when number.Value < 0:
                case NegateExpression _:
                    return $"{leftString}+({Visit((dynamic)addition.Right)})";
                default:
                    return $"{leftString}+{Visit((dynamic)addition.Right)}";
            }
        }
        #endregion
    }
}
