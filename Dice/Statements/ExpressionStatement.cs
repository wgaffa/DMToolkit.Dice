using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Statements
{
    public class ExpressionStatement : IStatement
    {
        public IExpression Expression { get; }

        public ExpressionStatement(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}