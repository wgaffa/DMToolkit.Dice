using System.Collections.Generic;
using System.Globalization;

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
            return $"<num: {Value.ToString(CultureInfo.InvariantCulture)}>";
        }
    }
}
