using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMTools
{
    public abstract class ValueObject<T> : IEquatable<T> where T : class
    {
        public abstract bool Equals(T other);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as T);
        }

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right) => EqualityComparer<ValueObject<T>>.Default.Equals(left, right);
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right) => !(left == right);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HashCodes()
                    .Aggregate(17, (hash, objectHashCode) => hash * 213 + objectHashCode);
                return hashCode;
            }
        }


        protected abstract IEnumerable<int> HashCodes();
    }
}
