using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationContext
    {
        public IExpression Expression { get; }
        public IExpression Result { get; set; }
        public ISymbolTable SymbolTable { get; set; }

        public DiceNotationContext(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }
    }
}
