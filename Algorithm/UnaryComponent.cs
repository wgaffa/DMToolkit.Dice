using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Algorithm
{
    public abstract class UnaryComponent : IComponent
    {
        public UnaryComponent(IComponent right)
        {
            this.right = right;
        }

        public abstract double Calculate();

        protected IComponent right;
    }
}
