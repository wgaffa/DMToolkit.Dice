using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public abstract class FunctionSymbol : ValueObject<FunctionSymbol>, ISymbol
    {
        protected readonly List<ISymbol> _parameters = new List<ISymbol>();

        public string Name { get; }
        public Maybe<ISymbol> Type { get; }
        public IReadOnlyList<ISymbol> Parameters => _parameters.AsReadOnly();

        protected FunctionSymbol(string name, Maybe<ISymbol> type)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(type, nameof(type));

            Name = name;
            Type = type;
        }

        protected FunctionSymbol(string name, Maybe<ISymbol> type, IEnumerable<ISymbol> parameters)
            : this(name, type)
        {
            Guard.Against.Null(parameters, nameof(parameters));

            _parameters = parameters.ToList();
        }

        public override bool Equals(FunctionSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name
                && Type.Equals(other.Type)
                && _parameters.SequenceEqual(other._parameters);
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return Name.GetHashCode();
            yield return Type.GetHashCode();
            foreach (var item in _parameters)
                yield return item.GetHashCode();
        }
    }
}