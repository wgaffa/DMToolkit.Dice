using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core
{
    public class DiceResult
    {
        public DiceResult(IEnumerable<int> collection)
        {
            _individualRolls = collection.ToList();
        }

        public int Result
        {
            get { return _individualRolls.Sum(); }
        }

        private readonly List<int> _individualRolls;

        public List<int> IndividualRolls
        {
            get { return _individualRolls; }
        }


    }
}
