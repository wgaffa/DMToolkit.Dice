using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Assignment : IExpression
    {
        public string Identifier { get; }
        public IExpression Expression { get; }
        public Maybe<Symbol> Symbol { get; internal set; } = None.Value;

        public Assignment(string identifier, IExpression expression)
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
