using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class VariableExpression : IExpression
    {
        public string Symbol { get; }

        public VariableExpression(string symbol)
        {
            Guard.Against.NullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"{Symbol}";
        }
    }
}
