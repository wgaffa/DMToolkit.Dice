using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class FunctionSymbol : ISymbol
    {
        public string Name { get; }

        public Maybe<ISymbol> Type { get; }

        public FunctionSymbol(string name, Maybe<ISymbol> type)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(type, nameof(type));

            Name = name;
            Type = type;
        }
    }
}
