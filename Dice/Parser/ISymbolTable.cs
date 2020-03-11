using Wgaffa.DMToolkit.Expressions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public interface ISymbolTable
    {
        IExpression this[string name] { get; }

        void Add(ISymbol symbol);
        Maybe<ISymbol> Lookup(string name);
    }
}
