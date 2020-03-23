using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DropExpression : UnaryExpression
    {
        public DropType Type { get; }

        public DropExpression(IExpression right) : base(right)
        {
            Type = DropType.Lowest;
        }

        public DropExpression(IExpression right, DropType type)
            : base(right)
        {
            Guard.Against.Null(type, nameof(type));

            Type = type;
        }

        public override string ToString()
        {
            return $"<drop: {Type.Name} {Right}>";
        }
    }
}
