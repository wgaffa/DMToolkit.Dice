using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class Divider : BinaryComponent
    {
        public Divider(IComponent left, IComponent right) : base(left, right)
        {
        }

        public override double Calculate()
        {
            return _left.Calculate() / _right.Calculate();
        }
    }
}
