using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public abstract class BinaryComponent : UnaryComponent
    {
        public BinaryComponent(IComponent left, IComponent right) : base(right)
        {
            this.left = left;
        }

        protected IComponent left;
    }
}
