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
            _globalScope = (ScopedSymbolTable)configuration.SymbolTable;
        }
        #endregion

        public Result<IExpression, IList<SemanticError>> Analyze(IExpression expression)
        {
            _errors.Clear();

            _currentScope = _globalScope;

            Visit((dynamic)expression);

            if (_errors.Count > 0)
                return _errors;
            else
                return Result<IExpression, IList<SemanticError>>.Ok(expression);
        }

        private IExpression Visit(IExpression expr)
            => expr;

        private IExpression Visit(BinaryExpression binary)
        {
            Visit((dynamic)binary.Left);
            Visit((dynamic)binary.Right);

            return binary;
        }

        private IExpression Visit(NegateExpression negate)
        {
            Visit((dynamic)negate.Right);

            return negate;
        }

        private IExpression Visit(CompoundExpression compound)
        {
            var list = compound.Expressions
                .Select(expr => (IExpression)Visit((dynamic)expr))
                .ToList();

            return compound;
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
                    _currentScope.Bind(s => s.LookupCurrent(v.Name))
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

            Visit((dynamic)assignment.Expression);
            assignment.Symbol = variableSymbol;

            return assignment;
        }

        private IExpression Visit(DefinitionExpression definition)
        {
            switch (_currentScope.Bind(s => s.Lookup(definition.Name)))
            {
                case Some<Symbol> some:
                    _errors.Add(SemanticError.VariableAlreadyDeclared(some.Reduce(default(Symbol)).Name));
                    break;

                case None<Symbol> none:
                    var definitionSymbol = new DefinitionSymbol(definition.Name, definition.Expression);
                    _currentScope.Match(s => s.Add(definitionSymbol), () => { });
                    definition.Symbol = definitionSymbol;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return definition;
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
            function.Symbol = userFunction;

            _currentScope.Bind(s => s.LookupCurrent(function.Identifier))
                .Match(
                ifSome: s => _errors.Add(SemanticError.VariableAlreadyDeclared(s.Name)),
                ifNone: () => _currentScope.Match(s => s.Add(userFunction), () => { }));

            int scopeLevel = _currentScope.Map(s => s.Level + 1).Reduce(1);
            _currentScope = new ScopedSymbolTable(_currentScope.Reduce(default(ScopedSymbolTable)), scopeLevel);

            parameters.ForEach(x => _currentScope.Do(scope => scope.Add(x)));
            Visit((dynamic)function.Body);

            _currentScope = _currentScope.Bind(s => s.EnclosingScope);

            return function;
        }

        private IExpression Visit(FunctionCallExpression functionCall)
        {
            functionCall.Symbol = _currentScope.Bind(s => s.Lookup(functionCall.Name));

            foreach (var argument in functionCall.Arguments)
            {
                Visit((dynamic)argument);
            }

            return functionCall;
        }
    }
}