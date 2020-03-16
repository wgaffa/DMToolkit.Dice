using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return $"{string.Join(';', _expressions)}";
        }
    }
}
