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

            Func<IExpression, string> negateLiteral = (expr) => $"-{Literal(expr)}";
            Func<IExpression, string> negateGroup = (expr) => OpenGroup(() => $"-({Literal(expr)})");

            _position++;
            if (groupPredicates.Any((f) => f()))
                return negateGroup(negate.Right);
            else
                return negateLiteral(negate.Right);
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

            string leftString;
            if (leftHandGroupPredicates.Any((f) => f(addition.Left)))
                leftString = Group(addition.Left);
            else
                leftString = Literal(addition.Left);

            _position++;
            string rightString;
            if (leftHandGroupPredicates.Any((f) => f(addition.Right)))
                rightString = Group(addition.Right);
            else
                rightString = Literal(addition.Right);

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
