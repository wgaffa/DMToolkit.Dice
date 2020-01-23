using System;
using System.Collections.Generic;
using System.Linq;

namespace DMTools.Die.Term
{
    public class TakeHighestPreserveOrderingTerm : IDiceTerm
    {
        private readonly IDiceTerm _diceTerm;
        private readonly int _takeAmount;

        public TakeHighestPreserveOrderingTerm(IDiceTerm diceTerm, int takeAmount = 1)
        {
            _diceTerm = diceTerm ?? throw new ArgumentNullException(nameof(diceTerm));
            _takeAmount = takeAmount > 0 ? takeAmount : 1;
        }

        public IEnumerable<int> GetResults()
        {
            List<int> takenResults = _diceTerm.GetResults().ToList();
            while(takenResults.Count > _takeAmount)
            {
                takenResults.Remove(takenResults.Min());
            }

            return takenResults;
        }
    }
}
