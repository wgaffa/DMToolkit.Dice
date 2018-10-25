using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class Subtracter : BinaryComponent
    {
        public Subtracter(IComponent left, IComponent right) : base(left, right)
        {
        }

        public override double Calculate()
        {
            return left.Calculate() - right.Calculate();
        }
    }
}
