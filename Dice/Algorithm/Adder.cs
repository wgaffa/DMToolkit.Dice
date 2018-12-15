using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Algorithm
{
    public class Adder : BinaryExpression
    {
        public Adder(IDiceExpression left, IDiceExpression right) : base(left, right)
        {
        }

        public override double Calculate()
        {
            return _left.Calculate() + _right.Calculate();
        }

        public override string ToString()
        {
            return _left.ToString() + " + " + _right.ToString();
        }
    }
}
