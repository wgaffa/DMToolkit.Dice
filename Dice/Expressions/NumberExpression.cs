using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Expressions
{
    public class NumberExpression : ValueObject<NumberExpression>, IExpression
    {
        public float Value { get; }

        public NumberExpression(float value)
        {
            Value = value;
        }

        public override bool Equals(NumberExpression other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return Value.GetHashCode();
        }
    }
}
