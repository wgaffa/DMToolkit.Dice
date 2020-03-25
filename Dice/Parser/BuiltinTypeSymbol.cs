using System;
using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class BuiltinTypeSymbol : Symbol, IEquatable<BuiltinTypeSymbol>
    {
        public BuiltinTypeSymbol(string name)
            : base(name, None.Value)
        {
        }

        public override string ToString()
        {
            return Name;
        }

        #region Equality
        public bool Equals(BuiltinTypeSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name
                && Type.Equals(other.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals(obj as BuiltinTypeSymbol);
        }

        public static bool operator ==(BuiltinTypeSymbol left, BuiltinTypeSymbol right) =>
            EqualityComparer<BuiltinTypeSymbol>.Default.Equals(left, right);

        public static bool operator !=(BuiltinTypeSymbol left, BuiltinTypeSymbol right) =>
            !(left == right);
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 213 + Name.GetHashCode();
                hash = hash * 213 + Type.GetHashCode();
                return hash;
            }
        }
    }
}