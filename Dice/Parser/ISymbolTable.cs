using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public interface ISymbolTable : IEnumerable<ISymbol>
    {
        int Depth { get; }
        void Add(ISymbol symbol);
        Maybe<ISymbol> Lookup(string name);
    }
}
