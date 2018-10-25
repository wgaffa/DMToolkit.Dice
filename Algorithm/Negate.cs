using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class Negate : UnaryComponent
    {
        public Negate(IComponent right) : base(right)
        {
        }
        
        public override double Calculate()
        {
            return -_right.Calculate();
        }

        public override string ToString()
        {
            return "-" + _right.ToString();
        }
    }
}
