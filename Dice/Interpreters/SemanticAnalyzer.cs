using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Interpreters.Errors;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class SemanticAnalyzer
    {
        private List<SemanticError> _errors = new List<SemanticError>();

        public Result<IExpression, IList<SemanticError>> Analyze(DiceNotationContext context)
        {
            _errors.Clear();

            var result = Visit((dynamic)context.Expression, context);

            if (_errors.Count > 0)
                return _errors;
            else
                return result;
        }

        private IExpression Visit(IExpression expr, DiceNotationContext context)
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
                .Nothing(() => throw new InvalidOperationException("undefined type"))
                .Map(type =>
                    varDecl.Names.Select(name => new VariableSymbol(name, Maybe<ISymbol>.Some(type))))
                .Map(vars => vars.Each(v =>
                    context.SymbolTable.Lookup(v.Name)
                    .Match(
                        ifSome: s => throw new InvalidOperationException($"duplicate identifier {s.Name}"),
                        ifNone: () => context.SymbolTable.Add(v))
                    ));

            return varDecl;
        }

        private IExpression Visit(VariableExpression variable, DiceNotationContext context)
        {
            var symbol = context.SymbolTable.Lookup(variable.Symbol)
                .Nothing(() => _errors.Add(new SemanticError("VAR", 1, "undefined variable")));

            return variable;
        }

        private IExpression Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            return assignment;
        }
    }
}
