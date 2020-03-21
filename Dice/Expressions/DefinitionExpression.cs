using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DefinitionExpression : IExpression
    {
        public string Name { get; }
        public IExpression Expression { get; }
        public Maybe<ISymbol> Symbol { get; }

        public DefinitionExpression(string name, IExpression expression)
            : this(name, expression, None.Value)
        {
        }

        public DefinitionExpression(string name, IExpression expression, Maybe<ISymbol> symbol)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(expression, nameof(expression));
            Guard.Against.Null(symbol, nameof(symbol));

            Name = name;
            Expression = expression;
            Symbol = symbol;
        }
    }
}