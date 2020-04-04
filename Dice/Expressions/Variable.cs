using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Variable : IExpression
    {
        public string Identifier { get; }
        public Maybe<Symbol> Symbol { get; internal set; }

        public Variable(string identifier)
            : this(identifier, None.Value)
        {
        }

        public Variable(string identifier, Maybe<Symbol> symbol)
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
