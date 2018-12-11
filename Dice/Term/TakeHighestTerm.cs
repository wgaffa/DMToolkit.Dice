using System;
using System.Collections.Generic;
using System.Linq;

namespace DMTools.Die.Term
{
    public class TakeHighestTerm : IDiceTerm
    {
        private IDiceTerm _diceTerm;
        private int _takeAmount;

        public TakeHighestTerm(IDiceTerm diceTerm, int dropAmount = 1)
        {
            _diceTerm = diceTerm ?? throw new ArgumentNullException(nameof(diceTerm));
            _takeAmount = dropAmount > 0 ? dropAmount : 1;
        }

        public IEnumerable<int> GetResults()
        {
            return _diceTerm.GetResults().OrderByDescending(r => r).Take(_takeAmount);
        }
    }
}
