using Ardalis.GuardClauses;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class BuiltinTypeSymbol : ISymbol
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
    }
}
