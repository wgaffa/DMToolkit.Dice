using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Drop : UnaryExpression
    {
        public DropType Type { get; }

        public Drop(IExpression right) : base(right)
        {
            Type = DropType.Lowest;
        }

        public Drop(IExpression right, DropType type)
            : base(right)
        {
            Guard.Against.Null(type, nameof(type));

            Type = type;
        }

        public override string ToString()
        {
            return $"<drop: {Type.Name} {Right}>";
        }
    }
}