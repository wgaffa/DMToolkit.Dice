using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public class Constant : IComponent
    {
        public Constant(double constant)
        {
            value = constant;
        }

        public double Calculate()
        {
            return value;
        }

        private double value;
    }
}
