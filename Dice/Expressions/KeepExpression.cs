using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class KeepExpression : UnaryExpression
    {
        public int Count { get; }

        public KeepExpression(IExpression right, int count)
            : base(right)
        {
            Guard.Against.Negative(count, nameof(count));

            Count = count;
        }

        public override string ToString()
        {
            return $"<keep: {Count} {Right}>";
        }
    }
}
