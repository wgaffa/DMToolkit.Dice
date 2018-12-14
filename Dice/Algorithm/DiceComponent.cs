using DMTools.Die.Term;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _currentRoll = _diceTerm.GetResults().ToList();
        }

        public double Calculate()
        {
            return _currentRoll.Sum();
        }

        public override string ToString()
        {
            Debug.Assert(_currentRoll != null);

            return "[" + String.Join(", ", _currentRoll) + "]";
        }

        private IDiceTerm _diceTerm;
        private List<int> _currentRoll;
    }
}
