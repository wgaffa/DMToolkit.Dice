using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class VariableExpression : IExpression
    {
        public string Identifier { get; }
        public Maybe<Symbol> Symbol { get; }

        public VariableExpression(string identifier)
            : this(identifier, None.Value)
        {
        }

        public VariableExpression(string identifier, Maybe<Symbol> symbol)
        {
            Guard.Against.NullOrWhiteSpace(identifier, nameof(identifier));
            Guard.Against.Null(symbol, nameof(symbol));

            Identifier = identifier;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"<var: {Identifier}>";
        }
    }
}
