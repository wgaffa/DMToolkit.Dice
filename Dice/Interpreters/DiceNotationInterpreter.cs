﻿using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wgaffa.DMToolkit.Exceptions;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;

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
            var diceRoller = context.DiceRoller ?? dice.DiceRoller;

            var rolls = Enumerable
                .Range(0, dice.NumberOfRolls)
                .Select(_ => diceRoller.RollDice(dice.Dice))
                .ToList();

            var rollsExpressions = rolls.Select(x => new NumberExpression(x));
            _expressionStack.Push(new RollResultExpression(rolls));

            return rolls.Sum();
        }

        private float Visit(VariableExpression variable, DiceNotationContext context)
        {
            if (context.SymbolTable is null)
                throw new SymbolTableUndefinedException("No symbol table is set");

            var symbolValue = context.SymbolTable[variable.Symbol]
                ?? throw new VariableUndefinedException(variable.Symbol, $"{variable.Symbol} is undefined");

            float result = Visit((dynamic)symbolValue, context);

            return result;
        }
        #endregion

        #region Unary Expressions
        private float Visit(NegateExpression negate, DiceNotationContext context)
        {
            float result = -Visit((dynamic)negate.Right, context);

            _expressionStack.Push(new NegateExpression(_expressionStack.Pop()));

            return result;
        }

        private float Visit(DropExpression drop, DiceNotationContext context)
        {
            Visit((dynamic)drop.Right, context);

            var lastResult = _expressionStack.Pop();
            if (lastResult is RollResultExpression roll)
            {
                var dropList = drop.Strategy(roll.Keep);
                var keepList = roll.Keep.Without(dropList);
                RollResultExpression newRoll = new RollResultExpression(
                    keepList,
                    roll.Discard.Concat(dropList));
                _expressionStack.Push(newRoll);

                return newRoll.Keep.Sum();
            }

            throw new InvalidOperationException();
        }

        private float Visit(KeepExpression keep, DiceNotationContext context)
        {
            Visit((dynamic)keep.Right, context);

            var lastResult = _expressionStack.Pop();
            if (lastResult is RollResultExpression roll)
            {
                var keepList = roll.Keep.OrderByDescending(x => x).Take(keep.Count).ToList();
                var dropped = roll.Keep.Without(keepList);

                var newList = new List<int>();
                foreach (var item in roll.Keep)
                {
                    if (keepList.Contains(item))
                    {
                        newList.Add(item);
                        keepList.Remove(item);
                    }
                }

                Debug.Assert(keepList.Count == 0);

                _expressionStack.Push(new RollResultExpression(
                    newList,
                    roll.Discard.AppendRange(dropped)));
                return newList.Sum();
            }

            throw new InvalidOperationException();
        }
        #endregion

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
