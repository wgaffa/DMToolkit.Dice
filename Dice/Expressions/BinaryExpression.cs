using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Expressions
{
    public abstract class BinaryExpression : UnaryExpression
    {
        public IExpression Left { get; }

        protected BinaryExpression(IExpression left, IExpression right) : base(right)
        {
            Guard.Against.Null(left, nameof(left));

            Left = left;
        }
    }
}
