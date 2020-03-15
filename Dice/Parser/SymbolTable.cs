using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class SymbolTable : ISymbolTable
    {
        private readonly Dictionary<string, ISymbol> _symbols = new Dictionary<string, ISymbol>();

        public int Depth => _symbols.Count;

        public SymbolTable()
        {
        }

        public SymbolTable(IEnumerable<ISymbol> builtInSymbols)
            => builtInSymbols.Each(s => Add(s));

        public void Add(ISymbol symbol) => _symbols.Add(symbol.Name, symbol);

        public Maybe<ISymbol> Lookup(string name)
            => _symbols.ContainsKey(name)
                ? Maybe<ISymbol>.Some(_symbols[name])
                : (Maybe<ISymbol>)None.Value;
    }
}
