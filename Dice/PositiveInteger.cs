using DMTools.Die;
using System;
using System.Collections.Generic;

namespace Wgaffa.DMToolkit
{
    public class PositiveInteger : ValueObject<PositiveInteger>
    {
        private readonly int _value;

        public PositiveInteger(int value = 1)
        {
            if (value < 1)
                throw new ArgumentException(ErrorMessages.InvalidPositiveInteger, nameof(value));

            _value = value;
        }

        public static explicit operator int(PositiveInteger value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value._value;
        }

        public static implicit operator PositiveInteger(int value) => new PositiveInteger(value);

        public static PositiveInteger ToPositiveInteger(int value) => new PositiveInteger(value);

        public int ToInt32() => _value;

        public override bool Equals(PositiveInteger other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other._value == _value;
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return _value;
        }
    }
}
