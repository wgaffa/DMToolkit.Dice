using Ardalis.GuardClauses;
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
        private ScopedSymbolTable _currentScope;
        private readonly Stack<RollResult> _rollResults = new Stack<RollResult>();
        private readonly Configuration _configuration;
        private readonly ScopedSymbolTable _globalSymbolTable;

        #region Constructors
        public DiceNotationInterpreter()
            : this(new ScopedSymbolTable())
        {}

        public DiceNotationInterpreter(ScopedSymbolTable symbolTable)
            : this(symbolTable, new Configuration())
        {
        }

        public DiceNotationInterpreter(ScopedSymbolTable symbolTable, Configuration configuration)
        {
            Guard.Against.Null(symbolTable, nameof(symbolTable));
            Guard.Against.Null(configuration, nameof(configuration));

            _configuration = configuration;
            _globalSymbolTable = symbolTable;
        }
        #endregion

        #region Public API

        public double Interpret(IExpression expression, IEnumerable<KeyValuePair<string, double>> initialValues = null)
        {
            Guard.Against.Null(expression, nameof(expression));

            var record = new ActivationRecord("main", RecordType.Program, 1);
            if (!(initialValues is null))
                initialValues.Each(kv => record[kv.Key] = kv.Value);

            _currentScope = _globalSymbolTable;

            _callStack.Push(record);

            var result = (double)Visit((dynamic)expression);

            _callStack.Pop();

            return result;
        }
        #endregion Public API

        #region Terminal expressions

        private double Visit(NumberExpression number)
        {
            return number.Value;
        }

        private double Visit(ListExpression list)
        {
            return list.Expressions.Aggregate(.0, (acc, expr) => acc + Visit((dynamic)expr));
        }

        private double Visit(DiceExpression dice)
        {
            var diceRoller = _configuration.DiceRoller ?? dice.DiceRoller;

            var rolls = Enumerable
                .Range(0, dice.NumberOfRolls)
                .Select(_ => diceRoller.RollDice(dice.Dice))
                .ToList();

            _rollResults.Push(new RollResult(rolls));

            return rolls.Sum();
        }

        private double Visit(VariableExpression variable)
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
                DefinitionSymbol def => (double)Visit((dynamic)def.Expression),
                _ => throw new InvalidOperationException()
            };
        }

        #endregion Terminal expressions

        #region Unary Expressions

        private double Visit(NegateExpression negate)
        {
            return (double)-Visit((dynamic)negate.Right);
        }

        private double Visit(DropExpression drop)
        {
            Visit((dynamic)drop.Right);

            var roll = _rollResults.Pop();

            if (roll is null)
                throw new InvalidOperationException("State of interpreter is out of sync, no rolls were made");

            var dropList = drop.Type.Strategy(roll.Keep);
            var keepList = roll.Keep.Without(dropList);
            RollResult newRoll = new RollResult(
                keepList,
                roll.Discard.Concat(dropList));
            _rollResults.Push(newRoll);

            return newRoll.Keep.Sum();
        }

        private double Visit(KeepExpression keep)
        {
            Visit((dynamic)keep.Right);

            var roll = _rollResults.Pop();

            if (roll is null)
                throw new InvalidOperationException("State of interpreter is out of sync, no rolls were made");

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

            _rollResults.Push(new RollResult(
                newList,
                roll.Discard.AppendRange(dropped)));
            return newList.Sum();
        }

        private double Visit(RepeatExpression repeat)
        {
            return (double)Enumerable
                .Range(0, repeat.RepeatTimes)
                .Aggregate(.0, (acc, _) => acc + Visit((dynamic)repeat.Right));
        }

        #endregion Unary Expressions

        #region Binary Expressions

        private double Visit(AdditionExpression addition)
        {
            return (double)(Visit((dynamic)addition.Left) + Visit((dynamic)addition.Right));
        }

        private double Visit(SubtractionExpression subtraction)
        {
            return (double)(Visit((dynamic)subtraction.Left) - Visit((dynamic)subtraction.Right));
        }

        private double Visit(MultiplicationExpression multiplication)
        {
            return (double)(Visit((dynamic)multiplication.Left) * Visit((dynamic)multiplication.Right));
        }

        private double Visit(DivisionExpression divition)
        {
            return (double)(Visit((dynamic)divition.Left) / Visit((dynamic)divition.Right));
        }

        #endregion Binary Expressions

        public double Visit(FunctionCallExpression function)
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
                    .Each(x => record[x.First.Name] = Visit((dynamic)x.Second))
                    .ToList());

            _callStack.Push(record);

            double result = 0;
            switch (castedSymbol.Reduce(default(FunctionSymbol)))
            {
                case BuiltinFunctionSymbol builtin:
                    result = builtin.Call(record);
                    break;

                case UserFunctionSymbol userFunction:
                    result = Visit((dynamic)userFunction.Body);
                    break;

                default:
                    break;
            }

            _callStack.Pop();

            return result;
        }

        private double Visit(VariableDeclarationExpression varDecl)
        {
            var record = _callStack.Peek();
            var value = varDecl.InitialValue
                .Map(expr => (double)Visit((dynamic)expr))
                .Map(v => varDecl.Names.Each(name => record[name] = v));

            return 0;
        }

        private double Visit(AssignmentExpression assignment)
        {
            double result = Visit((dynamic)assignment.Expression);

            var record = _callStack.Peek();
            int currentScopeLevel = record.NestingLevel;
            int variableScopeLevel = assignment.Symbol.Map(x => x.ScopeLevel).Reduce(currentScopeLevel);
            var declaredRecord = record.Follow(currentScopeLevel - variableScopeLevel);

            declaredRecord.Match(
                ifSome: x => x[assignment.Identifier] = result,
                ifNone: () => throw new InvalidOperationException());

            return result;
        }

        private double Visit(DefinitionExpression _)
        {
            return 0;
        }

        private double Visit(CompoundExpression compound)
        {
            double lastResult = 0;
            foreach (var block in compound.Expressions)
            {
                lastResult = Visit((dynamic)block);
            }

            return lastResult;
        }

        private double Visit(FunctionExpression _)
        {
            return 0;
        }
    }
}