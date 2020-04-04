namespace Wgaffa.DMToolkit.Expressions
{
    public class Addition : BinaryExpression
    {
        public Addition(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"<+ {Left} {Right}>";
        }
    }
}