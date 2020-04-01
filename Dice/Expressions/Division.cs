namespace Wgaffa.DMToolkit.Expressions
{
    public class Division : BinaryExpression
    {
        public Division(IExpression left, IExpression right) : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"</ {Left} {Right}>";
        }
    }
}
