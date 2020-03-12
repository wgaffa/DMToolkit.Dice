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
        public string Name { get; }
        public string Type { get; }

        public VariableDeclarationExpression(string name, string type)
        {
            Guard.Against.Null(name, nameof(name));
            Guard.Against.Null(type, nameof(type));

            Name = name;
            Type = type;
        }
    }
}
