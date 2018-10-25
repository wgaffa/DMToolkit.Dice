using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class Constant : IComponent
    {
        public Constant(double value)
        {
            _value = value;
        }

        public double Calculate()
        {
            return _value;
        }

        private double _value;
    }
}
