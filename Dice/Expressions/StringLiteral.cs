using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class StringLiteral : IExpression
    {
        public string Value { get; }

        public StringLiteral(string value)
        {
            Guard.Against.Null(value, nameof(value));

            Value = value;
        }
    }
}