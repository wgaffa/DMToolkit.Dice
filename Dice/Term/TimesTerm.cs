using System;
using System.Collections.Generic;
using System.Linq;

namespace DMTools.Die.Term
{
    public class TimesTerm : IDiceTerm
    {
        private IDiceTerm _dice;
        private int _numberOfRolls;

        public TimesTerm(IDiceTerm dice, int numberOfRolls = 1)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
            _numberOfRolls = numberOfRolls > 0 ? numberOfRolls : 1;
        }

        public IEnumerable<int> GetResults()
        {
            for (int i = 0; i < _numberOfRolls; i++)
            {
                yield return _dice.GetResults().First();
            }
        }
    }
}