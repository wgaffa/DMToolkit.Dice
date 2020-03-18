using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class UserFunctionSymbol : ISymbol
    {
        private readonly List<ISymbol> _parameters = new List<ISymbol>();

        public string Name { get; }

        public Maybe<ISymbol> Type { get; }

        public IExpression Body { get; }

        public IReadOnlyList<ISymbol> Parameters => _parameters.AsReadOnly();

        public UserFunctionSymbol(string name, Maybe<ISymbol> type, IExpression body)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(type, nameof(type));
            Guard.Against.Null(body, nameof(body));

            Name = name;
            Type = type;
            Body = body;
        }

        public UserFunctionSymbol(string name, Maybe<ISymbol> type, IExpression body, IEnumerable<ISymbol> parameters)
            : this(name, type, body)
        {
            _parameters = parameters.ToList();
        }
    }
}
