using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public interface ISymbolTable
    {
        int Depth { get; }
        void Add(ISymbol symbol);
        Maybe<ISymbol> Lookup(string name);
    }
}
