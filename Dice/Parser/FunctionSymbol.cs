using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class FunctionSymbol : Symbol, IEquatable<FunctionSymbol>
    {
        protected readonly List<Symbol> _parameters = new List<Symbol>();

        public IReadOnlyList<Symbol> Parameters => _parameters.AsReadOnly();

        public ICallable Implementation { get; }

        public FunctionSymbol(string name, Maybe<Symbol> type, ICallable implementation)
            : base(name, type)
        {
            Guard.Against.Null(implementation, nameof(implementation));

            Implementation = implementation;
        }

        public FunctionSymbol(string name, Maybe<Symbol> type, ICallable implementation, IEnumerable<Symbol> parameters)
            : this(name, type, implementation)
        {
            Guard.Against.Null(parameters, nameof(parameters));

            _parameters = parameters.ToList();
        }

        #region Equality
        public bool Equals(FunctionSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name
                && Type.Equals(other.Type)
                && _parameters.SequenceEqual(other._parameters)
                && EqualityComparer<ICallable>.Default.Equals(Implementation, other.Implementation);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals(obj as FunctionSymbol);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 213 + Name.GetHashCode();
                hash = hash * 213 + Type.GetHashCode();
                hash = hash * 213 + _parameters.Aggregate(hash, (acc, symbol) => acc * 213 + symbol.GetHashCode());
                hash = hash * 213 + Implementation.GetHashCode();

                return hash;
            }
        }
    }
}