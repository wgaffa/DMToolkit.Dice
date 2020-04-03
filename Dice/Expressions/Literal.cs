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
            return $"<literal: {Value}>";
        }
    }
}