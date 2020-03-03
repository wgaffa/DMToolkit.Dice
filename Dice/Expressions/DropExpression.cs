namespace Wgaffa.DMToolkit.Expressions
{
    public enum DropType
    {
        Lowest,
        Highest,
    }

    public class DropExpression : UnaryExpression
    {
        public DropType Type { get; } = DropType.Lowest;

        public DropExpression(IExpression right) : base(right)
        {
        }

        public DropExpression(IExpression right, DropType dropType)
            : base(right)
        {
            Type = dropType;
        }
    }
}
