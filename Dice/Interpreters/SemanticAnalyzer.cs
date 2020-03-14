using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class SemanticAnalyzer
    {
        public IExpression Analyze(DiceNotationContext context)
        {
            return Visit((dynamic)context.Expression, context);
        }

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
                        ifSome: _ => throw new InvalidOperationException("duplicate identifier"),
                        ifNone: () => context.SymbolTable.Add(v))
                    ));

            return varDecl;
        }

        private IExpression Visit(VariableExpression variable, DiceNotationContext context)
        {
            return variable;
        }

        private IExpression Visit(AssignmentExpression assignment, DiceNotationContext context)
        {
            return assignment;
        }
    }
}
