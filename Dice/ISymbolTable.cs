using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit
{
    public interface ISymbolTable
    {
        IExpression this[string symbol] { get; }
    }
}
