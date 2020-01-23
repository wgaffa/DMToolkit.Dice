using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Algorithm
{
    public class Multiplier : BinaryExpression
    {
        public Multiplier(IDiceExpression left, IDiceExpression right) : base(left, right)
        {
        }

        public override double Calculate()
        {
            return Left.Calculate() * Right.Calculate();
        }

        public override string ToString()
        {
            return Left.ToString() + " * " + Right.ToString();
        }
    }
}
