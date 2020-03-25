using Ardalis.GuardClauses;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public abstract class Symbol
    {
        public string Name { get; }
        public Maybe<Symbol> Type { get; }
        public int ScopeLevel { get; internal set; }

        protected Symbol(string name, Maybe<Symbol> type, int scopeLevel = 0)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(type, nameof(type));

            Name = name;
            Type = type;
            ScopeLevel = scopeLevel;
        }
    }
}