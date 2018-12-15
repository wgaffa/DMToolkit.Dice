using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Algorithm
{
    public abstract class UnaryExpression : IDiceExpression
    {
        public UnaryExpression(IDiceExpression right)
        {
            _right = right;
        }

        public abstract double Calculate();

        protected IDiceExpression _right;
    }
}
