namespace Wgaffa.DMToolkit.Expressions
{
    public class SubtractionExpression : BinaryExpression
    {
        public SubtractionExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"-({Left}, {Right})";
        }
    }
}
