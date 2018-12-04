using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core.Algorithm
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

        public override string ToString()
        {
            return _left.ToString() + " / " + _right.ToString();
        }
    }
}
