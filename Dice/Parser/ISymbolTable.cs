using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public interface ISymbolTable : IEnumerable<Symbol>
    {
        int Depth { get; }
        void Add(Symbol symbol);
        Maybe<Symbol> Lookup(string name);
    }
}
