using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Expression
{
    public class Constant : IDiceExpression
    {
        public Constant(int constant)
        {
            _constant = constant;
        }

        public int Evaluate()
        {
            return _constant;
        }

        private readonly int _constant;
    }
}
