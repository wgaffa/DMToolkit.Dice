using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Expressions
{
    public class VariableDeclarationExpression : IExpression
    {
        private readonly List<string> _names = new List<string>();

        public IReadOnlyList<string> Names { get; }
        public string Type { get; }

        public VariableDeclarationExpression(IEnumerable<string> names, string type)
        {
            Guard.Against.Null(names, nameof(names));
            Guard.Against.Null(type, nameof(type));

            _names = names.ToList();
            Type = type;
        }
    }
}
