using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wgaffa.DMToolkit.Exceptions;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationInterpreter
    {
        private readonly Dictionary<string, double> _globalMemory = new Dictionary<string, double>();

        #region Public API
        public double Interpret(DiceNotationContext context, Maybe<IEnumerable<KeyValuePair<string, double>>> initialValues = null)
        {
            Guard.Against.Null(context, nameof(context));
            initialValues
                .NoneIfNull()
                .Match(
                    ifSome: x => x.Each(kv => _globalMemory[kv.Key] = kv.Value),
                    ifNone: () => { });

            double total = Visit((dynamic)context.Expression, context);

            return total;
        }

        public double Interpret(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            double total = Visit((dynamic)expression, new DiceNotationContext(expression));

            return total;
        }
        #endregion

        #region Terminal expressions
        private double Visit(NumberExpression number, DiceNotationContext _)
        {
            return number.Value;
        }

        private double Visit(ListExpression list, DiceNotationContext context)
        {
            double sumOfList = list.Expressions.Aggregate(.0, (acc, expr) => acc + Visit((dynamic)expr, context));

            return sumOfList;
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
            if (context.SymbolTable is null)
                throw new SymbolTableUndefinedException("No symbol table is set");

            var symbolValue = context.SymbolTable.Lookup(variable.Symbol)
                .Nothing(() => throw new VariableUndefinedException(variable.Symbol, $"{variable.Symbol} is undefined"))
                .Map(v => _globalMemory[v.Name])
                .Reduce(default(double));

            return symbolValue;
        }
        #endregion

        #region Unary Expressions
        private double Visit(NegateExpression negate, DiceNotationContext context)
        {
            double result = -Visit((dynamic)negate.Right, context);

            return result;
        }

        private double Visit(DropExpression drop, DiceNotationContext context)
        {
            Visit((dynamic)drop.Right, context);

            if (context.RollResults.Last() is RollResult roll)
            {
                var dropList = drop.Strategy(roll.Keep);
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
            double result = Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(.0, (acc, _) => acc + Visit((dynamic)repeat.Right, context));

            return result;
        }
        #endregion

        #region Binary Expressions
        private double Visit(AdditionExpression addition, DiceNotationContext context)
        {
            double result = Visit((dynamic)addition.Left, context) + Visit((dynamic)addition.Right, context);

            return result;
        }

        private double Visit(SubtractionExpression subtraction, DiceNotationContext context)
        {
            double result = Visit((dynamic)subtraction.Left, context) - Visit((dynamic)subtraction.Right, context);

            return result;
        }

        private double Visit(MultiplicationExpression multiplication, DiceNotationContext context)
        {
            double result = Visit((dynamic)multiplication.Left, context) * Visit((dynamic)multiplication.Right, context);

            return result;
        }

        private double Visit(DivisionExpression divition, DiceNotationContext context)
        {
            double result = Visit((dynamic)divition.Left, context) / Visit((dynamic)divition.Right, context);

            return result;
        }
        #endregion

        public double Visit(FunctionCallExpression function, DiceNotationContext context)
        {
            var functionSymbol = context.SymbolTable
                .Lookup(function.Name)
                .Bind(sym =>
                sym is FunctionSymbol fsym
                ? Maybe<FunctionSymbol>.Some(fsym)
                : (Maybe<FunctionSymbol>)None.Value);

            var arguments = functionSymbol
                .Map(funcSym => funcSym.Parameters)
                .Map(parameters => parameters.Zip(
                    function.Arguments,
                    (sym, expr) => new { Param = sym, Arg = (double)Visit((dynamic)expr, context) })
                    .ToList());

            var result = (float)arguments
                .Map(args => args.Select(x => x.Arg))
                .Bind(args => functionSymbol.Map(f => f.Call(args)))
                .Reduce(default(double));

            return result;
        }

        private double Visit(VariableDeclarationExpression varDecl, DiceNotationContext context)
        {
            var value = varDecl.InitialValue
                .Map(expr => (double)Visit((dynamic)expr, context))
                .Map(v => varDecl.Names.Each(name => _globalMemory[name] = v));

            return 0;
        }

        private double Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            double result = Visit((dynamic)assignment.Expression, context);

            _globalMemory[assignment.Identifier] = result;

            return result;
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
    }
}
