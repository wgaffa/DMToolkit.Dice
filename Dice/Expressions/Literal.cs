using System.Globalization;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Literal : IExpression
    {
        public object Value { get; }

        public Literal(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            var value = Value.GetType() == typeof(double)
                ? ((double)Value).ToString(CultureInfo.InvariantCulture)
                : Value.ToString();
            return $"<literal: {value}>";
        }
    }
}