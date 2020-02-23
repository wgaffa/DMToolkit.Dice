using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Extensions;

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

        public override string ToString()
        {
            return _expressions
                .Select(expr => expr.ToString())
                .StringJoin(", ")
                .SurroundWith("[]");
        }
    }
}
