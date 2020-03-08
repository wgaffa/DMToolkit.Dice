namespace Wgaffa.DMToolkit.Expressions
{
    public class MultiplicationExpression : BinaryExpression
    {
        public MultiplicationExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"Mul {Left}, {Right}";
        }
    }
}
