using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core.Algorithm
{
    public abstract class UnaryComponent : IComponent
    {
        public UnaryComponent(IComponent right)
        {
            _right = right;
        }

        public abstract double Calculate();

        protected IComponent _right;
    }
}
