using Ardalis.SmartEnum;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DropType : SmartEnum<DropType>
    {
        public static readonly DropType Lowest = new DropType("lowest", 1, rolls => new int[] { rolls.Min() });
        public static readonly DropType Highest = new DropType("highest", 1, rolls => new int[] { rolls.Max() });

        public delegate IEnumerable<int> StrategyFunc(IEnumerable<int> rolls);

        public StrategyFunc Strategy { get; }

        private DropType(string name, int value, StrategyFunc strategy) : base(name, value)
        {
            Strategy = strategy;
        }
    }
}