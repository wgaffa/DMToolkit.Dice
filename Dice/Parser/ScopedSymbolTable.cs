using Ardalis.GuardClauses;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class ScopedSymbolTable : ISymbolTable
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public int Depth => _symbols.Count;
        public int Level { get; }
        public Maybe<ISymbolTable> EnclosingScope { get; }

        public ScopedSymbolTable(Maybe<ISymbolTable> enclosingScope = null, int scopeLevel = 1)
        {
            EnclosingScope = enclosingScope.NoneIfNull();
            Level = scopeLevel;
        }

        public ScopedSymbolTable(IEnumerable<Symbol> builtinSymbols, Maybe<ISymbolTable> enclosingScope = null, int scopeLevel = 1)
            : this(enclosingScope, scopeLevel)
        {
            Guard.Against.Null(builtinSymbols, nameof(builtinSymbols));

            builtinSymbols.Each(Add);
        }

        public void Add(Symbol symbol)
        {
            symbol.ScopeLevel = Level;
            _symbols.Add(symbol.Name, symbol);
        }

        public Maybe<Symbol> Lookup(string name)
            => _symbols.ContainsKey(name)
                ? Maybe<Symbol>.Some(_symbols[name])
                : EnclosingScope.Bind(s => s.Lookup(name));

        public Maybe<Symbol> LookupCurrent(string name)
            => _symbols.ContainsKey(name)
                ? Maybe<Symbol>.Some(_symbols[name])
                : (Maybe<Symbol>)None.Value;

        public IEnumerator<Symbol> GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }
    }
}