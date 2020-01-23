using System;
using System.Collections.Generic;
using System.Linq;

namespace DMTools.Die.Term
{
    public class DropLowestPreserveOrderingTerm : IDiceTerm
    {
        private readonly IDiceTerm _diceTerm;
        private readonly int _dropAmount;

        public DropLowestPreserveOrderingTerm(IDiceTerm diceTerm, int dropAmount = 1)
        {
            _diceTerm = diceTerm ?? throw new ArgumentNullException(nameof(diceTerm));
            _dropAmount = dropAmount > 0 ? dropAmount : 1;
        }

        public IEnumerable<int> GetResults()
        {
            List<int> takenResults = _diceTerm.GetResults().ToList();

            for (int i = 0; i < _dropAmount; i++)
            {
                if (takenResults.Count == 0)
                    break;

                takenResults.Remove(takenResults.Min());
            }

            return takenResults;
        }
    }
}