using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Expression
{
    public class BinaryOperation : IDiceExpression
    {
        public BinaryOperation(Func<int, int, int> func, IDiceExpression lhs, IDiceExpression rhs)
        {
            _func = func;
            _lhs = lhs;
            _rhs = rhs;
        }

        public int Evaluate()
        {
            return _func(_lhs.Evaluate(), _rhs.Evaluate());
        }

        private readonly Func<int, int, int> _func;
        private readonly IDiceExpression _lhs;
        private readonly IDiceExpression _rhs;
    }
}
