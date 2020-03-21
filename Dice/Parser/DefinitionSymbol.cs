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
    public class DefinitionSymbol : ValueObject<DefinitionSymbol>, ISymbol
    {
        public string Name { get; }

        public IExpression Expression { get; }

        public Maybe<ISymbol> Type => None.Value;

        public DefinitionSymbol(string name, IExpression expression)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(expression, nameof(expression));

            Name = name;
            Expression = expression;
        }

        public override bool Equals(DefinitionSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name;
        }

        protected override IEnumerable<int> HashCodes()
        {
            yield return Name.GetHashCode();
        }
    }
}
