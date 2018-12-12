using DMTools.Die.Term;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Algorithm
{
    public class DiceComponent : IComponent
    {
        public DiceComponent(IDiceTerm diceTerm)
        {
            _diceTerm = diceTerm ?? throw new ArgumentNullException(nameof(diceTerm));
        }

        public double Calculate()
        {
            return _diceTerm.GetResults().Sum();
        }

        public override string ToString()
        {
            return "[" + String.Join(", ", _diceTerm.GetResults()) + "]";
        }

        private IDiceTerm _diceTerm;
    }
}
