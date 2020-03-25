using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class VariableSymbol : Symbol, IEquatable<VariableSymbol>
    {
        public VariableSymbol(string name, Maybe<Symbol> type)
            : base(name, type)
        {
            Guard.Against.NullOrNone(type, nameof(type));
        }

        public override string ToString()
        {
            return $"{Name}:{Type}";
        }

        #region Equality
        public bool Equals(VariableSymbol other)
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

            return Equals(obj as VariableSymbol);
        }

        public static bool operator ==(VariableSymbol left, VariableSymbol right) =>
            EqualityComparer<VariableSymbol>.Default.Equals(left, right);

        public static bool operator !=(VariableSymbol left, VariableSymbol right) =>
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
