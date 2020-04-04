using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Statements
{
    public class Return : IStatement
    {
        public IExpression Expression { get; }

        public Return(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }
    }
}