using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class NumberListExpression : IExpression
    {
        private readonly List<float> _values;

        public IReadOnlyList<float> Values => _values.AsReadOnly();

        public NumberListExpression(IEnumerable<float> values)
        {
            Guard.Against.Null(values, nameof(values));

            _values = values.ToList();
        }
    }
}
