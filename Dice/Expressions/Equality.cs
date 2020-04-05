namespace Wgaffa.DMToolkit.Expressions
{
    public enum EqualityComparison
    {
        Equality,
        Inequality,
    }

    public class Equality : BinaryExpression
    {
        public EqualityComparison Comparison { get; }

        public Equality(IExpression left, IExpression right) : base(left, right)
        {
        }

        public Equality(IExpression left, IExpression right, EqualityComparison compare)
            : base(left, right)
        {
            Comparison = compare;
        }
    }
}