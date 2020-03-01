namespace Wgaffa.DMToolkit.Expressions
{
    public class DivisionExpression : BinaryExpression
    {
        public DivisionExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"Div{Left}, {Right}";
        }
    }
}
