using Ardalis.GuardClauses;
using System;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class VariableSymbol : ISymbol
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
    }
}
