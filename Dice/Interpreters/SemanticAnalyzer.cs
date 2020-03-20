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

        private IExpression Visit(BinaryExpression binary, DiceNotationContext context)
        {
            Visit((dynamic)binary.Left, context);
            Visit((dynamic)binary.Right, context);

            return binary;
        }

        private IExpression Visit(UnaryExpression unary, DiceNotationContext context)
        {
            Visit((dynamic)unary.Right, context);

            return unary;
        }

        private IExpression Visit(CompoundExpression compound, DiceNotationContext context)
        {
            compound.Expressions.Each(expr => Visit((dynamic)expr, context));

            return compound;
        }

        private IExpression Visit(VariableDeclarationExpression varDecl, DiceNotationContext context)
        {
            var typeSymbol = context.SymbolTable.Lookup(varDecl.Type);

            var symbols = typeSymbol
                .Nothing(() => _errors.Add(SemanticError.VariableUnknownType(varDecl.Type)))
                .Map(type =>
                    varDecl.Names.Select(name => new VariableSymbol(name, Maybe<ISymbol>.Some(type))))
                .Map(vars => vars.Each(v =>
                    context.SymbolTable.Lookup(v.Name)
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
            var symbol = context.SymbolTable.Lookup(variable.Symbol)
                .Nothing(() => _errors.Add(SemanticError.VariableUndefined(variable.Symbol)));

            return variable;
        }

        private IExpression Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            var identifier = context.SymbolTable.Lookup(assignment.Identifier)
                .Nothing(() => _errors.Add(SemanticError.VariableUndefined(assignment.Identifier)));

            Visit((dynamic)assignment.Expression, context);

            return assignment;
        }

        private IExpression Visit(DefinitionExpression definition, DiceNotationContext context)
        {
            context.SymbolTable.Lookup(definition.Name)
                .Match(
                    ifSome: s => _errors.Add(SemanticError.VariableAlreadyDeclared(s.Name)),
                    ifNone: () => _currentScope.Match(
                        s => s.Add(new DefinitionSymbol(definition.Name, definition.Expression)),
                        () => { }));

            return definition;
        }

        private IExpression Visit(FunctionExpression function, DiceNotationContext context)
        {
            var userFunction = new UserFunctionSymbol(
                function.Identifier,
                context.SymbolTable.Lookup(function.ReturnType),
                function.Body);

            context.SymbolTable.Lookup(function.Identifier)
                .Match(
                ifSome: s => SemanticError.VariableAlreadyDeclared(s.Name),
                ifNone: () => context.SymbolTable.Add(userFunction));

            int scopeLevel = _currentScope.Map(s => ((ScopedSymbolTable)s).Level + 1).Reduce(1);
            _currentScope = new ScopedSymbolTable(_currentScope, scopeLevel);

            Visit((dynamic)function.Body, context);

            _currentScope = _currentScope.Bind(s => ((ScopedSymbolTable)s).EnclosingScope);

            return function;
        }
    }
}