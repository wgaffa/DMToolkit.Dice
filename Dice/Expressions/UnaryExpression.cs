using System;

namespace Wgaffa.DMToolkit.Expressions
{
    public abstract class UnaryExpression : IExpression
    {
        public IExpression Right { get; }

        public UnaryExpression(IExpression right)
        {
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }
}
