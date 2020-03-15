using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class VariableSymbol : ValueObject<VariableSymbol>, ISymbol
    {
        public string Name { get; }

        public Maybe<ISymbol> Type { get; }

        public VariableSymbol(string name, Maybe<ISymbol> type)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.NullOrNone(type, nameof(type));

            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name}:{Type}";
        }

        public override bool Equals(VariableSymbol other)
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
