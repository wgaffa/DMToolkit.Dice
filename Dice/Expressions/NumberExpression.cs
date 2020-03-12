using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Expressions
{
    public class NumberExpression : IExpression
    {
        public double Value { get; }

        public NumberExpression(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}
