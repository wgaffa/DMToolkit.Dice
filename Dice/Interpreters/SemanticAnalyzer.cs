using Ardalis.GuardClauses;
using System;
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
        private readonly ScopedSymbolTable _globalScope;
        private Maybe<ScopedSymbolTable> _currentScope = None.Value;
        private readonly Configuration _configuration;

        #region Constructors
        public SemanticAnalyzer(Configuration configuration)
        {
            Guard.Against.Null(configuration, nameof(configuration));

            _configuration = configuration;
            _globalScope = new ScopedSymbolTable(configuration.SymbolTable);
        }
        #endregion

        public Result<IExpression, IList<SemanticError>> Analyze(IExpression expression)
        {
            _errors.Clear();

            _currentScope = _globalScope;

            var result = Visit((dynamic)expression);

            if (_errors.Count > 0)
                return _errors;
            else
                return result;
        }

        private IExpression Visit(IExpression expr)
            => expr;

        private IExpression Visit(AdditionExpression addition)
            => new AdditionExpression(
                Visit((dynamic)addition.Left),
                Visit((dynamic)addition.Right));

        private IExpression Visit(SubtractionExpression subtraction)
            => new SubtractionExpression(
                Visit((dynamic)subtraction.Left),
                Visit((dynamic)subtraction.Right));

        private IExpression Visit(MultiplicationExpression multiplication)
                    => new MultiplicationExpression(
                        Visit((dynamic)multiplication.Left),
                        Visit((dynamic)multiplication.Right));

        private IExpression Visit(DivisionExpression division)
                    => new DivisionExpression(
                        Visit((dynamic)division.Left),
                        Visit((dynamic)division.Right));

        private IExpression Visit(NegateExpression negate) =>
            new NegateExpression(Visit((dynamic)negate.Right));

        private IExpression Visit(CompoundExpression compound)
        {
            var list = compound.Expressions
                .Select(expr => (IExpression)Visit((dynamic)expr));

            return new CompoundExpression(list);
        }

        private IExpression Visit(VariableDeclarationExpression varDecl)
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
                .Map(expr => Visit((dynamic)expr));

            return varDecl;
        }

        private IExpression Visit(VariableExpression variable)
        {
            var symbol = _currentScope.Bind(s => s.Lookup(variable.Identifier));
            symbol.Match(
                ifSome: s => variable.Symbol = s,
                ifNone: () => _errors.Add(SemanticError.VariableUndefined(variable.Identifier)));

            return variable;
        }

        private IExpression Visit(AssignmentExpression assignment)
        {
            Maybe<Symbol> variableSymbol = None.Value;
            _currentScope.Bind(s => s.Lookup(assignment.Identifier))
                .Match(
                ifSome: s => variableSymbol = s,
                ifNone: () => _errors.Add(SemanticError.VariableUndefined(assignment.Identifier)));

            IExpression expr = Visit((dynamic)assignment.Expression);

            return new AssignmentExpression(assignment.Identifier, expr) { Symbol = variableSymbol };
        }

        private IExpression Visit(DefinitionExpression definition)
        {
            switch (_currentScope.Bind(s => s.Lookup(definition.Name)))
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

        private IExpression Visit(FunctionExpression function)
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

            var userFunction = new FunctionSymbol(
                function.Identifier,
                _currentScope.Bind(s => s.Lookup(function.ReturnType)),
                new UserFunction(function),
                parameters);

            _currentScope.Bind(s => s.Lookup(function.Identifier))
                .Match(
                ifSome: s => SemanticError.VariableAlreadyDeclared(s.Name),
                ifNone: () => _currentScope.Match(s => s.Add(userFunction), () => { }));

            int scopeLevel = _currentScope.Map(s => s.Level + 1).Reduce(1);
            _currentScope = new ScopedSymbolTable(_currentScope.Reduce(default(ScopedSymbolTable)), scopeLevel);

            parameters.ForEach(x => _currentScope.Do(scope => scope.Add(x)));
            IExpression body = Visit((dynamic)function.Body);

            _currentScope = _currentScope.Bind(s => s.EnclosingScope);

            return function;
        }

        private IExpression Visit(FunctionCallExpression functionCall)
        {
            Maybe<Symbol> functionSymbol = _currentScope.Bind(s => s.Lookup(functionCall.Name));
            var callExpression = new FunctionCallExpression(
                functionCall.Name,
                functionCall.Arguments
                    .Select(arg => (IExpression)Visit((dynamic)arg)));

            callExpression.Symbol = functionSymbol;

            return callExpression;
        }
    }
}