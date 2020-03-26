﻿using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationInterpreter
    {
        private readonly Stack<ActivationRecord> _callStack = new Stack<ActivationRecord>();
        private ISymbolTable _currentScope;

        #region Public API

        public double Interpret(DiceNotationContext context, IEnumerable<KeyValuePair<string, double>> initialValues = null)
        {
            Guard.Against.Null(context, nameof(context));
            Guard.Against.Null(context.SymbolTable, nameof(context.SymbolTable));

            var record = new ActivationRecord("main", RecordType.Program, 1);
            if (!(initialValues is null))
                initialValues.Each(kv => record[kv.Key] = kv.Value);

            _currentScope = context.SymbolTable;

            _callStack.Push(record);

            var result = (double)Visit((dynamic)context.Expression, context);

            _callStack.Pop();

            return result;
        }

        public double Interpret(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            return (double)Visit((dynamic)expression, new DiceNotationContext(expression));
        }

        #endregion Public API

        #region Terminal expressions

        private double Visit(NumberExpression number, DiceNotationContext _)
        {
            return number.Value;
        }

        private double Visit(ListExpression list, DiceNotationContext context)
        {
            return list.Expressions.Aggregate(.0, (acc, expr) => acc + Visit((dynamic)expr, context));
        }

        private double Visit(DiceExpression dice, DiceNotationContext context)
        {
            var diceRoller = context.DiceRoller ?? dice.DiceRoller;

            var rolls = Enumerable
                .Range(0, dice.NumberOfRolls)
                .Select(_ => diceRoller.RollDice(dice.Dice))
                .ToList();

            context.RollResults.Add(new RollResult(rolls));

            return rolls.Sum();
        }

        private double Visit(VariableExpression variable, DiceNotationContext context)
        {
            var record = _callStack.Peek();
            int currentScopeLevel = record.NestingLevel;
            int variableScope = variable.Symbol.Map(s => s.ScopeLevel).Reduce(currentScopeLevel);
            return variable.Symbol.Reduce(default(Symbol)) switch
            {
                VariableSymbol var => (double)record
                    .Follow(currentScopeLevel - variableScope)
                    .Map(x => x[var.Name])
                    .Reduce((double)0),
                DefinitionSymbol def => (double)Visit((dynamic)def.Expression, context),
                _ => throw new InvalidOperationException()
            };
        }

        #endregion Terminal expressions

        #region Unary Expressions

        private double Visit(NegateExpression negate, DiceNotationContext context)
        {
            return (double)-Visit((dynamic)negate.Right, context);
        }

        private double Visit(DropExpression drop, DiceNotationContext context)
        {
            Visit((dynamic)drop.Right, context);

            if (context.RollResults.Last() is RollResult roll)
            {
                var dropList = drop.Type.Strategy(roll.Keep);
                var keepList = roll.Keep.Without(dropList);
                RollResult newRoll = new RollResult(
                    keepList,
                    roll.Discard.Concat(dropList));
                context.RollResults.Add(newRoll);

                return newRoll.Keep.Sum();
            }

            throw new InvalidOperationException();
        }

        private double Visit(KeepExpression keep, DiceNotationContext context)
        {
            Visit((dynamic)keep.Right, context);

            if (context.RollResults.Last() is RollResult roll)
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

                context.RollResults.Add(new RollResult(
                    newList,
                    roll.Discard.AppendRange(dropped)));
                return newList.Sum();
            }

            throw new InvalidOperationException();
        }

        private double Visit(RepeatExpression repeat, DiceNotationContext context)
        {
            return (double)Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(.0, (acc, _) => acc + Visit((dynamic)repeat.Right, context));
        }

        #endregion Unary Expressions

        #region Binary Expressions

        private double Visit(AdditionExpression addition, DiceNotationContext context)
        {
            return (double)(Visit((dynamic)addition.Left, context) + Visit((dynamic)addition.Right, context));
        }

        private double Visit(SubtractionExpression subtraction, DiceNotationContext context)
        {
            return (double)(Visit((dynamic)subtraction.Left, context) - Visit((dynamic)subtraction.Right, context));
        }

        private double Visit(MultiplicationExpression multiplication, DiceNotationContext context)
        {
            return (double)(Visit((dynamic)multiplication.Left, context) * Visit((dynamic)multiplication.Right, context));
        }

        private double Visit(DivisionExpression divition, DiceNotationContext context)
        {
            return (double)(Visit((dynamic)divition.Left, context) / Visit((dynamic)divition.Right, context));
        }

        #endregion Binary Expressions

        public double Visit(FunctionCallExpression function, DiceNotationContext context)
        {
            Maybe<ActivationRecord> accesslink = None.Value;
            if (_callStack.Peek().NestingLevel < function.Symbol.Map(x => x.ScopeLevel).Reduce(0))
            {
                accesslink = _callStack.Peek().AccessLink;
            }
            else
            {
                int currentScope = _callStack.Peek().NestingLevel;
                int variableScope = function.Symbol.Map(s => s.ScopeLevel).Reduce(currentScope);
                accesslink = _callStack.Peek().Follow(currentScope - variableScope);
            }

            var record = new ActivationRecord(
                function.Name,
                RecordType.Function,
                function.Symbol.Map(x => x.ScopeLevel + 1).Reduce(0),
                accesslink);

            var castedSymbol = function.Symbol.Map(s => s as FunctionSymbol);
            var arguments = castedSymbol
                .Map(funcSym => funcSym.Parameters)
                .Map(parameters => parameters
                    .Zip(function.Arguments)
                    .Each(x => record[x.First.Name] = Visit((dynamic)x.Second, context))
                    .ToList());

            _callStack.Push(record);

            double result = 0;
            switch (castedSymbol.Reduce(default(FunctionSymbol)))
            {
                case BuiltinFunctionSymbol builtin:
                    result = builtin.Call(record);
                    break;

                case UserFunctionSymbol userFunction:
                    result = Visit((dynamic)userFunction.Body, context);
                    break;

                default:
                    break;
            }

            _callStack.Pop();

            return result;
        }

        private double Visit(VariableDeclarationExpression varDecl, DiceNotationContext context)
        {
            var record = _callStack.Peek();
            var value = varDecl.InitialValue
                .Map(expr => (double)Visit((dynamic)expr, context))
                .Map(v => varDecl.Names.Each(name => record[name] = v));

            return 0;
        }

        private double Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            double result = Visit((dynamic)assignment.Expression, context);

            var record = _callStack.Peek();
            int currentScopeLevel = record.NestingLevel;
            int variableScopeLevel = assignment.Symbol.Map(x => x.ScopeLevel).Reduce(currentScopeLevel);
            var declaredRecord = record.Follow(currentScopeLevel - variableScopeLevel);

            declaredRecord.Match(
                ifSome: x => x[assignment.Identifier] = result,
                ifNone: () => throw new InvalidOperationException());

            return result;
        }

        private double Visit(DefinitionExpression _, DiceNotationContext _1)
        {
            return 0;
        }

        private double Visit(CompoundExpression compound, DiceNotationContext context)
        {
            double lastResult = 0;
            foreach (var block in compound.Expressions)
            {
                lastResult = Visit((dynamic)block, context);
            }

            return lastResult;
        }

        private double Visit(FunctionExpression _, DiceNotationContext _1)
        {
            return 0;
        }
    }
}