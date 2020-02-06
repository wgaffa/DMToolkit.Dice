using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Expressions
{
    public class ListExpression : IExpression
    {
        private readonly List<IExpression> _expressions = new List<IExpression>();

        public IReadOnlyList<IExpression> Expressions => _expressions.AsReadOnly();

        public ListExpression(IEnumerable<IExpression> expressions)
        {
            Guard.Against.Null(expressions, nameof(expressions));

            _expressions = expressions.ToList();
        }
    }
}
