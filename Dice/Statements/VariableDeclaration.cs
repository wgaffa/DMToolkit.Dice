using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Statements
{
    public class VariableDeclaration : IStatement
    {
        private readonly List<string> _names = new List<string>();

        public IReadOnlyList<string> Names => _names.AsReadOnly();
        public string Type { get; }
        public Maybe<IExpression> InitialValue { get; }

        public VariableDeclaration(IEnumerable<string> names, string type, Maybe<IExpression> initialValue = null)
        {
            Guard.Against.Null(names, nameof(names));
            Guard.Against.Null(type, nameof(type));

            InitialValue = initialValue.NoneIfNull();
            _names = names.ToList();
            Type = type;
        }

        public override string ToString()
        {
            var assign = InitialValue.Map(expr => $" assign={expr}").Reduce(string.Empty);
            return $"<decl_var: type={Type} names={string.Join(", ", _names)}{assign}>";
        }
    }
}