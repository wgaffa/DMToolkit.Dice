using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class CompoundExpression : IExpression
    {
        private readonly List<IExpression> _expressions;

        public IReadOnlyList<IExpression> Expressions => _expressions.AsReadOnly();

        public CompoundExpression(IEnumerable<IExpression> expressions)
        {
            Guard.Against.Null(expressions, nameof(expressions));

            _expressions = expressions.ToList();
        }

        public override string ToString()
        {
            return $"<block: {string.Join(' ', _expressions)}>";
        }
    }
}