using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;

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

        private string Visit(VariableExpression variable)
        {
            _position++;
            return $"{variable}";
        }
        #endregion

        #region Unary NonTerminal
        private string Visit(NegateExpression negate)
        {
            var groupPredicates = new List<Func<bool>>()
            {
                () => negate.Right is BinaryExpression,
                () => negate.Right is NegateExpression,
                () => negate.Right is RepeatExpression,
                () => negate.Right is NumberExpression number && number.Value < 0
            };

            _position++;
            return GroupIfNecessary(negate.Right, groupPredicates, negateGroup, negateLiteral);

            string negateLiteral(IExpression expr) => $"-{Literal(expr)}";
            string negateGroup(IExpression expr) => OpenGroup(() => $"-({Literal(expr)})");
        }
        #endregion

        #region Binary NonTerminal
        private string Visit(AdditionExpression addition)
        {
            var groupPredicates = new List<Func<IExpression, bool>>()
            {
                (expr) => expr is NegateExpression && _position > 0,
                (expr) => expr is NumberExpression number && number.Value < 0 && _position > 0,
                (expr) => expr is RepeatExpression && _position > 0,
            };

            return BinaryDisplay(addition.Left, addition.Right, groupPredicates, "+");
        }

        private string Visit(SubtractionExpression subtract)
        {
            var groupPredicates = new List<Func<IExpression, bool>>()
            {
                (expr) => expr is NegateExpression && _position > 0,
                (expr) => expr is NumberExpression number && number.Value < 0 && _position > 0,
                (expr) => expr is RepeatExpression && _position > 0,
            };

            return BinaryDisplay(subtract.Left, subtract.Right, groupPredicates, "-");
        }

        private string Visit(MultiplicationExpression multiply)
        {
            var groupConditions = new List<Func<IExpression, bool>>()
            {
                (expr) => expr is AdditionExpression || expr is SubtractionExpression,
                (expr) => expr is NegateExpression && _position > 0,
                (expr) => expr is NumberExpression number && number.Value < 0 && _position > 0,
                (expr) => expr is RepeatExpression && _position > 0,
            };

            return BinaryDisplay(multiply.Left, multiply.Right, groupConditions, "*");
        }

        private string Visit(DivisionExpression division)
        {
            var groupConditions = new List<Func<IExpression, bool>>()
            {
                (expr) => expr is AdditionExpression || expr is SubtractionExpression,
                (expr) => expr is NegateExpression && _position > 0,
                (expr) => expr is NumberExpression number && number.Value < 0 && _position > 0,
                (expr) => expr is RepeatExpression && _position > 0,
            };

            return BinaryDisplay(division.Left, division.Right, groupConditions, "/");
        }
        #endregion

        #region Special Terminals
        private string Visit(ListExpression list)
        {
            var listExpressions = list.Expressions
                .Select(expr => OpenGroup(() => Literal(expr)))
                .StringJoin(", ")
                .SurroundWith("[]");
            _position++;

            return listExpressions;
        }

        private string Visit(RepeatExpression repeat)
        {
            var groupConditions = new List<Func<bool>>()
            {
                () => repeat.Right is BinaryExpression && repeat.RepeatTimes > 1,
                () => repeat.Right is NegateExpression && repeat.RepeatTimes > 1,
                () => repeat.Right is NumberExpression number && number.Value < 0,
            };

            return $"{repeatTimes()}{GroupIfNecessary(repeat.Right, groupConditions)}";

            string repeatTimes() => repeat.RepeatTimes > 1 ? $"{repeat.RepeatTimes}x" : string.Empty;
        }
        #endregion

        #region Private Functors
        private string GroupIfNecessary(IExpression expr, IEnumerable<Func<bool>> predicates) =>
            GroupIfNecessary(expr, predicates, Group, Literal);

        private string GroupIfNecessary(
            IExpression expr,
            IEnumerable<Func<bool>> predicates,
            Func<IExpression, string> group,
            Func<IExpression, string> literal)
        {
            return predicates.Any(p => p()) ? group(expr) : literal(expr);
        }

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

        private string BinaryDisplay(IExpression left, IExpression right, IEnumerable<Func<IExpression, bool>> predicates, string between)
        {
            return GroupIfNecessary(left, TransformPredicate(left, predicates))
                + increasePosition()
                + GroupIfNecessary(right, TransformPredicate(right, predicates));

            string increasePosition() { _position++; return between; }
        }

        private IEnumerable<Func<bool>> TransformPredicate(IExpression expr, IEnumerable<Func<IExpression, bool>> predicates) =>
            predicates.Select<Func<IExpression, bool>, Func<bool>>(f => () => f(expr));
        #endregion
    }
}
