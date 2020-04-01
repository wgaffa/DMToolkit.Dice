namespace Wgaffa.DMToolkit.Expressions
{
    public class Multiplication : BinaryExpression
    {
        public Multiplication(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"<* {Left} {Right}>";
        }
    }
}
