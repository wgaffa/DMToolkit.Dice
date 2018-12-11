using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core.DiceTerm
{
    public class DropLowestTerm
    {
        private IDiceTerm _diceTerm;
        private int _dropAmount;

        public DropLowestTerm(IDiceTerm diceTerm, int dropAmount = 1)
        {
            _diceTerm = diceTerm ?? throw new ArgumentNullException(nameof(diceTerm));
            _dropAmount = dropAmount > 0 ? dropAmount : 1;
        }

        public IEnumerable<int> GetResults()
        {
            return _diceTerm.GetResults().OrderBy(r => r).Skip(_dropAmount);
        }
    }
}
