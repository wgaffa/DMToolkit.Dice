using System.Globalization;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Number : IExpression
    {
        public double Value { get; }

        public Number(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<num: {Value.ToString(CultureInfo.InvariantCulture)}>";
        }
    }
}