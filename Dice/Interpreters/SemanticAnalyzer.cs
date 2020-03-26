﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Interpreters.Errors;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class SemanticAnalyzer
    {
        private readonly List<SemanticError> _errors = new List<SemanticError>();
        private ScopedSymbolTable _globalScope;
        private Maybe<ISymbolTable> _currentScope = None.Value;

        public Result<IExpression, IList<SemanticError>> Analyze(DiceNotationContext context)
        {
            _errors.Clear();

            _globalScope = new ScopedSymbolTable(context.SymbolTable, None.Value, 1);
            _currentScope = _globalScope;

            var result = Visit((dynamic)context.Expression, context);

            if (_errors.Count > 0)
                return _errors;
            else
                return result;
        }

        private IExpression Visit(IExpression expr, DiceNotationContext _)
            => expr;

        private IExpression Visit(AdditionExpression addition, DiceNotationContext context)
            => new AdditionExpression(
                Visit((dynamic)addition.Left, context),
                Visit((dynamic)addition.Right, context));

        private IExpression Visit(SubtractionExpression subtraction, DiceNotationContext context)
            => new SubtractionExpression(
                Visit((dynamic)subtraction.Left, context),
                Visit((dynamic)subtraction.Right, context));

        private IExpression Visit(MultiplicationExpression multiplication, DiceNotationContext context)
                    => new MultiplicationExpression(
                        Visit((dynamic)multiplication.Left, context),
                        Visit((dynamic)multiplication.Right, context));

        private IExpression Visit(DivisionExpression division, DiceNotationContext context)
                    => new DivisionExpression(
                        Visit((dynamic)division.Left, context),
                        Visit((dynamic)division.Right, context));

        private IExpression Visit(NegateExpression negate, DiceNotationContext context) =>
            new NegateExpression(Visit((dynamic)negate.Right, context));

        private IExpression Visit(CompoundExpression compound, DiceNotationContext context)
        {
            var list = compound.Expressions
                .Select(expr => (IExpression)Visit((dynamic)expr, context));

            return new CompoundExpression(list);
        }

        private IExpression Visit(VariableDeclarationExpression varDecl, DiceNotationContext context)
        {
            var typeSymbol = _currentScope.Bind(
                s => s.Lookup(varDecl.Type));

            var symbols = typeSymbol
                .Nothing(() => _errors.Add(SemanticError.VariableUnknownType(varDecl.Type)))
                .Map(type =>
                    varDecl.Names.Select(name => new VariableSymbol(name, Maybe<Symbol>.Some(type))))
                .Map(vars => vars.Each(v =>
                    _currentScope.Bind(s => ((ScopedSymbolTable)s).LookupCurrent(v.Name))
                    .Match(
                        ifSome: s => _errors.Add(SemanticError.VariableAlreadyDeclared(s.Name)),
                        ifNone: () => _currentScope.Match(s => s.Add(v), () => { }))
                    ));

            varDecl.InitialValue
                .Map(expr => Visit((dynamic)expr, context));

            return varDecl;
        }

        private IExpression Visit(VariableExpression variable, DiceNotationContext context)
        {
            var symbol = _currentScope.Bind(s => s.Lookup(variable.Identifier));
            symbol.Match(
                ifSome: s => variable.Symbol = s,
                ifNone: () => _errors.Add(SemanticError.VariableUndefined(variable.Identifier)));

            return variable;
        }

        private IExpression Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            Maybe<Symbol> variableSymbol = None.Value;
            _currentScope.Bind(s => s.Lookup(assignment.Identifier))
                .Match(
                ifSome: s => variableSymbol = s,
                ifNone: () => _errors.Add(SemanticError.VariableUndefined(assignment.Identifier)));

            IExpression expr = Visit((dynamic)assignment.Expression, context);

            return new AssignmentExpression(assignment.Identifier, expr) { Symbol = variableSymbol };
        }

        private IExpression Visit(DefinitionExpression definition, DiceNotationContext context)
        {
            var symbol = _currentScope.Bind(s => s.Lookup(definition.Name));

            switch (symbol)
            {
                case Some<Symbol> some:
                    _errors.Add(SemanticError.VariableAlreadyDeclared(some.Reduce(default(Symbol)).Name));
                    return definition;

                case None<Symbol> none:
                    var definitionSymbol = new DefinitionSymbol(definition.Name, definition.Expression);
                    _currentScope.Match(s => s.Add(definitionSymbol), () => { });
                    return new DefinitionExpression(
                        definition.Name,
                        definition.Expression,
                        Maybe<Symbol>.Some(definitionSymbol));

                default:
                    throw new InvalidOperationException();
            }
        }

        private IExpression Visit(FunctionExpression function, DiceNotationContext context)
        {
            var parameters = function.Parameters
                .Select(x => new { Id = x.Value, Symbol = _currentScope.Bind(s => s.Lookup(x.Key)) })
                .Where(x => x.Symbol is Some<Symbol>)
                .Select(x => new VariableSymbol(x.Id, x.Symbol))
                .ToList();

            if (parameters.Count != function.Parameters.Count)
            {
                _errors.Add(SemanticError.VariableUnknownType(string.Empty));
            }

            var userFunction = new UserFunctionSymbol(
                function.Identifier,
                _currentScope.Bind(s => s.Lookup(function.ReturnType)),
                function.Body,
                parameters);

            _currentScope.Bind(s => s.Lookup(function.Identifier))
                .Match(
                ifSome: s => SemanticError.VariableAlreadyDeclared(s.Name),
                ifNone: () => _currentScope.Match(s => s.Add(userFunction), () => { }));

            int scopeLevel = _currentScope.Map(s => ((ScopedSymbolTable)s).Level + 1).Reduce(1);
            _currentScope = new ScopedSymbolTable(_currentScope, scopeLevel);

            parameters.ForEach(x => _currentScope.Do(scope => scope.Add(x)));
            IExpression body = Visit((dynamic)function.Body, context);

            userFunction.Body = body;

            _currentScope = _currentScope.Bind(s => ((ScopedSymbolTable)s).EnclosingScope);

            return new FunctionExpression(
                function.Identifier,
                body,
                function.ReturnType,
                function.Parameters);
        }

        private IExpression Visit(FunctionCallExpression functionCall, DiceNotationContext context)
        {
            Maybe<Symbol> functionSymbol = _currentScope.Bind(s => s.Lookup(functionCall.Name));
            var callExpression = new FunctionCallExpression(
                functionCall.Name,
                functionCall.Arguments
                    .Select(arg => (IExpression)Visit((dynamic)arg, context)));

            callExpression.Symbol = functionSymbol;

            return callExpression;
        }
    }
}