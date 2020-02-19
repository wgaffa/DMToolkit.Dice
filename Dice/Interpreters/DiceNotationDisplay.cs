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
            var groupPredicates = new List<Func<bool>>()
            {
                () => negate.Right is AdditionExpression,
                () => negate.Right is NegateExpression,
                () => negate.Right is NumberExpression number && number.Value < 0
            };

            _position++;
            return groupPredicates.Any((f) => f())
                ? negateGroup(negate.Right)
                : negateLiteral(negate.Right);

            string negateLiteral(IExpression expr) => $"-{Literal(expr)}";
            string negateGroup(IExpression expr) => OpenGroup(() => $"-({Literal(expr)})");
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

            string leftString = leftHandGroupPredicates.Any((f) => f(addition.Left))
                ? Group(addition.Left)
                : Literal(addition.Left);

            _position++;
            string rightString = leftHandGroupPredicates.Any((f) => f(addition.Right))
                ? Group(addition.Right)
                : Literal(addition.Right);

            return $"{leftString}+{rightString}";
        }
        #endregion

        #region Private Functors
        private string Group(IExpression expr) => OpenGroup(() => $"({Literal(expr)})");
        private Action RestorePosition(int x) => () => _position = x;
        private Action StorePosition() => RestorePosition(_position);
        private void ResetPosition() => _position = 0;

        private string OpenGroup(Func<string> func)
        {
            var restorePosition = StorePosition();
            ResetPosition();
            string result = func();
            restorePosition();
            return result;
        }

        private string Literal(IExpression expr) => $"{Visit((dynamic)expr)}";
        #endregion
    }
}
