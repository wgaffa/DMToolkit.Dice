using Ardalis.GuardClauses;
using DMTools.Die.Rollers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationInterpreter
    {
        private readonly Stack<IExpression> _expressionStack = new Stack<IExpression>();
        private ISymbolTable _symbolTable;

        public DiceNotationInterpreter()
        {
        }

        #region Public API
        public float Interpret(DiceNotationContext context)
        {
            Guard.Against.Null(context, nameof(context));

            Debug.Assert(_expressionStack.Count == 0);

            _symbolTable = context.SymbolTable;
            float total = Visit((dynamic)context.Expression);
            context.Result = _expressionStack.Pop();

            Debug.Assert(_expressionStack.Count == 0);

            _symbolTable = null;

            return total;
        }

        public float Interpret(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Debug.Assert(_expressionStack.Count == 0);

            float total = Visit((dynamic)expression);
            _ = _expressionStack.Pop();

            Debug.Assert(_expressionStack.Count == 0);

            return total;
        }
        #endregion

        #region Terminal expressions
        private float Visit(NumberExpression number)
        {
            _expressionStack.Push(number);

            return number.Value;
        }

        private float Visit(ListExpression list)
        {
            float sumOfList = list.Expressions.Aggregate(0f, (acc, expr) => acc + Visit((dynamic)expr));

            var expressionList = Enumerable
                .Range(0, list.Expressions.Count)
                .Select(_ => _expressionStack.Pop())
                .ToList();

            _expressionStack.Push(new ListExpression(expressionList));

            return sumOfList;
        }

        private float Visit(DiceExpression dice)
        {
            var rolls = Enumerable
                .Range(0, dice.NumberOfRolls)
                .Select(_ => (float)dice.DiceRoller.RollDice(dice.Dice))
                .ToList();

            var rollsExpressions = rolls.Select(x => new NumberExpression(x));
            _expressionStack.Push(new ListExpression(rollsExpressions));

            return rolls.Sum();
        }

        private float Visit(VariableExpression variable)
        {
            var evaluated = _symbolTable[variable.Symbol];

            float result = Visit((dynamic)evaluated);

            return result;
        }
        #endregion

        private float Visit(NegateExpression negate)
        {
            float result = -Visit((dynamic)negate.Right);

            _expressionStack.Push(new NegateExpression(_expressionStack.Pop()));

            return result;
        }

        private float Visit(RepeatExpression repeat)
        {
            float result = Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(0f, (acc, _) => acc + Visit((dynamic)repeat.Right));

            var list = Enumerable
                .Range(0, repeat.RepeatTimes)
                .Select(_ => _expressionStack.Pop())
                .ToList();

            _expressionStack.Push(new ListExpression(list));

            return result;
        }

        #region Binary Expressions
        private float Visit(AdditionExpression addition)
        {
            float result = Visit((dynamic)addition.Left) + Visit((dynamic)addition.Right);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new AdditionExpression(left, right));

            return result;
        }

        private float Visit(SubtractionExpression subtraction)
        {
            float result = Visit((dynamic)subtraction.Left) - Visit((dynamic)subtraction.Right);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new SubtractionExpression(left, right));

            return result;
        }

        private float Visit(MultiplicationExpression multiplication)
        {
            float result = Visit((dynamic)multiplication.Left) * Visit((dynamic)multiplication.Right);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new MultiplicationExpression(left, right));

            return result;
        }

        private float Visit(DivisionExpression divition)
        {
            float result = Visit((dynamic)divition.Left) / Visit((dynamic)divition.Right);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new DivisionExpression(left, right));

            return result;
        }
        #endregion
    }
}
