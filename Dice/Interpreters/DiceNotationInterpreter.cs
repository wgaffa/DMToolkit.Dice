using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wgaffa.DMToolkit.Exceptions;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationInterpreter
    {
        private readonly Stack<IExpression> _expressionStack = new Stack<IExpression>();

        #region Public API
        public float Interpret(DiceNotationContext context)
        {
            Guard.Against.Null(context, nameof(context));

            Debug.Assert(_expressionStack.Count == 0);

            float total = Visit((dynamic)context.Expression, context);
            context.Result = _expressionStack.Pop();

            Debug.Assert(_expressionStack.Count == 0);

            return total;
        }

        public float Interpret(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Debug.Assert(_expressionStack.Count == 0);

            float total = Visit((dynamic)expression, new DiceNotationContext(expression));
            _ = _expressionStack.Pop();

            Debug.Assert(_expressionStack.Count == 0);

            return total;
        }
        #endregion

        #region Terminal expressions
        private float Visit(NumberExpression number, DiceNotationContext _)
        {
            _expressionStack.Push(number);

            return number.Value;
        }

        private float Visit(ListExpression list, DiceNotationContext context)
        {
            float sumOfList = list.Expressions.Aggregate(0f, (acc, expr) => acc + Visit((dynamic)expr, context));

            var expressionList = Enumerable
                .Range(0, list.Expressions.Count)
                .Select(_ => _expressionStack.Pop())
                .ToList();

            _expressionStack.Push(new ListExpression(expressionList));

            return sumOfList;
        }

        private float Visit(DiceExpression dice, DiceNotationContext context)
        {
            var rolls = Enumerable
                .Range(0, dice.NumberOfRolls)
                .Select(_ => (float)dice.DiceRoller.RollDice(dice.Dice))
                .ToList();

            var rollsExpressions = rolls.Select(x => new NumberExpression(x));
            _expressionStack.Push(new ListExpression(rollsExpressions));

            return rolls.Sum();
        }

        private float Visit(VariableExpression variable, DiceNotationContext context)
        {
            if (context.SymbolTable is null)
                throw new SymbolTableUndefinedException("No symbol table is set");

            var symbolValue = context.SymbolTable[variable.Symbol]
                ?? throw new VariablUndefinedException(variable.Symbol, $"{variable.Symbol} is undefined");

            float result = Visit((dynamic)symbolValue, context);

            return result;
        }
        #endregion

        private float Visit(NegateExpression negate, DiceNotationContext context)
        {
            float result = -Visit((dynamic)negate.Right, context);

            _expressionStack.Push(new NegateExpression(_expressionStack.Pop()));

            return result;
        }

        private float Visit(RepeatExpression repeat, DiceNotationContext context)
        {
            float result = Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(0f, (acc, _) => acc + Visit((dynamic)repeat.Right, context));

            var list = Enumerable
                .Range(0, repeat.RepeatTimes)
                .Select(_ => _expressionStack.Pop())
                .ToList();

            _expressionStack.Push(new ListExpression(list));

            return result;
        }

        #region Binary Expressions
        private float Visit(AdditionExpression addition, DiceNotationContext context)
        {
            float result = Visit((dynamic)addition.Left, context) + Visit((dynamic)addition.Right, context);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new AdditionExpression(left, right));

            return result;
        }

        private float Visit(SubtractionExpression subtraction, DiceNotationContext context)
        {
            float result = Visit((dynamic)subtraction.Left, context) - Visit((dynamic)subtraction.Right, context);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new SubtractionExpression(left, right));

            return result;
        }

        private float Visit(MultiplicationExpression multiplication, DiceNotationContext context)
        {
            float result = Visit((dynamic)multiplication.Left, context) * Visit((dynamic)multiplication.Right, context);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new MultiplicationExpression(left, right));

            return result;
        }

        private float Visit(DivisionExpression divition, DiceNotationContext context)
        {
            float result = Visit((dynamic)divition.Left, context) / Visit((dynamic)divition.Right, context);

            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(new DivisionExpression(left, right));

            return result;
        }
        #endregion
    }
}
