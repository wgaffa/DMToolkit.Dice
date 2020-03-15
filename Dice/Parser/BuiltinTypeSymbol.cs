using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class BuiltinTypeSymbol : ValueObject<BuiltinTypeSymbol>, ISymbol
    {
        public string Name { get; }

        public Maybe<ISymbol> Type => None.Value;

        public BuiltinTypeSymbol(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(BuiltinTypeSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name
                && Type.Equals(other.Type);
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return Name.GetHashCode();
            yield return Type.GetHashCode();
        }
    }
}
