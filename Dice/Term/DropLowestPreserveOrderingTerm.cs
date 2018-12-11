using System;
using System.Collections.Generic;
using System.Linq;

namespace DMTools.Core.DiceTerm
{
    public class DropLowestPreserveOrderingTerm : IDiceTerm
    {
        private IDiceTerm _diceTerm;
        private int _dropAmount;

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
                takenResults.Remove(takenResults.Min());
            }

            return takenResults;
        }
    }
}