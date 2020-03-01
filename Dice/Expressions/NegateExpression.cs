namespace Wgaffa.DMToolkit.Expressions
{
    public class NegateExpression : UnaryExpression
    {
        public NegateExpression(IExpression right)
            : base(right)
        {
        }

        public override string ToString()
        {
            return $"Neg {Right}";
        }
    }
}
