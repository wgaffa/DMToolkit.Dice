using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class AssignmentExpression : IExpression
    {
        public string Identifier { get; }
        public IExpression Expression { get; }

        public AssignmentExpression(string identifier, IExpression expression)
        {
            Guard.Against.NullOrWhiteSpace(identifier, nameof(identifier));
            Guard.Against.Null(expression, nameof(expression));

            Identifier = identifier;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"<assign: var={Identifier} value={Expression}>";
        }
    }
}
