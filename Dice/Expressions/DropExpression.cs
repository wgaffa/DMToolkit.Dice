using System;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DropExpression : UnaryExpression
    {
        public delegate IEnumerable<int> StrategyFunc(IEnumerable<int> rolls);
        public StrategyFunc Strategy { get; }

        public DropExpression(IExpression right) : base(right)
        {
            Strategy = x => new int[] { x.Min() };
        }

        public DropExpression(IExpression right, StrategyFunc strategy)
            : base(right)
        {
            Strategy = strategy;
        }
    }
}
