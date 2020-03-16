using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class VariableDeclarationExpression : IExpression
    {
        private readonly List<string> _names = new List<string>();

        public IReadOnlyList<string> Names => _names.AsReadOnly();
        public string Type { get; }
        public Maybe<IExpression> InitialValue { get; }

        public VariableDeclarationExpression(IEnumerable<string> names, string type, Maybe<IExpression> initialValue = null)
        {
            Guard.Against.Null(names, nameof(names));
            Guard.Against.Null(type, nameof(type));

            InitialValue = initialValue.NoneIfNull();
            _names = names.ToList();
            Type = type;
        }
    }
}