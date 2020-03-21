using Ardalis.GuardClauses;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class ScopedSymbolTable : ISymbolTable
    {
        private readonly Dictionary<string, ISymbol> _symbols = new Dictionary<string, ISymbol>();

        public int Depth => _symbols.Count;
        public int Level { get; }
        public Maybe<ISymbolTable> EnclosingScope { get; }

        public ScopedSymbolTable(Maybe<ISymbolTable> enclosingScope = null, int scopeLevel = 1)
        {
            EnclosingScope = enclosingScope.NoneIfNull();
            Level = scopeLevel;
        }

        public ScopedSymbolTable(IEnumerable<ISymbol> builtinSymbols, Maybe<ISymbolTable> enclosingScope = null, int scopeLevel = 1)
            : this(enclosingScope, scopeLevel)
        {
            Guard.Against.Null(builtinSymbols, nameof(builtinSymbols));

            builtinSymbols.Each(s => Add(s));
        }

        public void Add(ISymbol symbol) => _symbols.Add(symbol.Name, symbol);

        public Maybe<ISymbol> Lookup(string name)
            => _symbols.ContainsKey(name)
                ? Maybe<ISymbol>.Some(_symbols[name])
                : EnclosingScope.Bind(s => s.Lookup(name));

        public Maybe<ISymbol> LookupCurrent(string name)
            => _symbols.ContainsKey(name)
                ? Maybe<ISymbol>.Some(_symbols[name])
                : (Maybe<ISymbol>)None.Value;

        public IEnumerator<ISymbol> GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }
    }
}