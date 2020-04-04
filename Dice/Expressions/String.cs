using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class String : IExpression
    {
        public string Value { get; }

        public String(string value)
        {
            Guard.Against.Null(value, nameof(value));

            Value = value;
        }
    }
}