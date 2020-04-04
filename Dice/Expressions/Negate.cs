namespace Wgaffa.DMToolkit.Expressions
{
    public class Negate : UnaryExpression
    {
        public Negate(IExpression right)
            : base(right)
        {
        }

        public override string ToString()
        {
            return $"<negate {Right}>";
        }
    }
}
