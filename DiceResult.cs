using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice
{
    public class DiceResult
    {
        public DiceResult(int result, IEnumerable<int> collection)
        {
            _result = result;
            _individualRolls = collection.ToList();
        }

        private readonly int _result;

        public int Result
        {
            get { return _result; }
        }

        private readonly List<int> _individualRolls;

        public List<int> IndividualRolls
        {
            get { return _individualRolls; }
        }


    }
}
