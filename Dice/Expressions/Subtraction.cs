namespace Wgaffa.DMToolkit.Expressions
{
    public class Subtraction : BinaryExpression
    {
        public Subtraction(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"<- {Left} {Right}>";
        }
    }
}
