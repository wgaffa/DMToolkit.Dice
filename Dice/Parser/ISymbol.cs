using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public interface ISymbol
    {
        string Name { get; }
        Maybe<ISymbol> Type { get; }
    }
}
