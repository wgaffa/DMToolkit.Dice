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

            Func<IExpression, string> literal = (expr) => $"{Visit((dynamic)expr)}";
            Func<IExpression, string> negateLiteral = (expr) => $"-{literal(expr)}";
            Func<int, Action> restorePosition = (x) => () => _position = x;
            Action resetPosition = () => _position = 0;
            Func<Action> storePosition = () => { var point = restorePosition(_position); resetPosition(); return point; };
            Func<Func<string>, string> openGroup = (func) => { var restorePoint = storePosition(); string r = func(); restorePoint(); return r; };
            Func<IExpression, string> group = (expr) => openGroup(() => $"-({literal(expr)})");

            _position++;
            if (groupPredicates.Any((f) => f()))
                return group(negate.Right);
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

            Func<IExpression, string> literal = (expr) => $"{Visit((dynamic)expr)}";
            Func<int, Action> restorePosition = (x) => () => _position = x;
            Action resetPosition = () => _position = 0;
            Func<Action> storePosition = () => { var point = restorePosition(_position); resetPosition(); return point; };
            Func<Func<string>, string> openGroup = (func) => { var restorePoint = storePosition(); string r = func(); restorePoint(); return r; };
            Func<IExpression, string> group = (expr) => openGroup(() => $"({literal(expr)})");

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
