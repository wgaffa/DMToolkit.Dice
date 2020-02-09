using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Expressions
{
    public class NumberExpression : IExpression
    {
        public float Value { get; }

        public NumberExpression(float value)
        {
            Value = value;
        }
    }
}
